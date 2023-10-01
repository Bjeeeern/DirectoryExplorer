using Game.Primitives;
using Game.Utility.Extensions;
using Game.Services.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Game.Utility;
using Microsoft.Extensions.DependencyInjection;
using Game.Services.Providers;

namespace Game
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphicsDeviceManager;
        private ServiceProvider serviceProvider;

        private List<IEntity> entities = new ();
        private Dictionary<string, Texture2D> textureDict = new ();
        private Dictionary<string, SpriteFont> fontDict = new ();
        private Matrix cameraOffset;

        public Game()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphicsDeviceManager.IsFullScreen = false;
            graphicsDeviceManager.SetScreenScale(1920, 1080);
            graphicsDeviceManager.ApplyChanges();

            cameraOffset = Matrix.CreateWorld(graphicsDeviceManager.GetScreenScale() * 0.5f, Vector3.Forward, Vector3.Up);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textureDict["line"] = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            textureDict["line"].SetData(new[] { Color.White }, 0, 1);

            fontDict["default"] = Content.Load<SpriteFont>("fonts/default");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDirectoryReader, DirectoryReader>();
            serviceCollection.AddSingleton<IWorldBuilder, WorldBuilder>();
            serviceCollection.AddSingleton<RoomBuilder>();
            serviceCollection.AddSingleton(fontDict["default"]);

            serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

            entities = serviceProvider
                .GetRequiredService<IWorldBuilder>()
                .BuildWorld();

            entities
                .Where<ISprite>()
                .DistinctBy(x => x.TextureName)
                .Do(s => textureDict[s.TextureName] = Content.Load<Texture2D>($"images/{s.TextureName}"))
                .Enumerate();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            var direction = new Vector2
            {
                X = (keyboardState.IsAnyKeyDown(Keys.Left, Keys.A) ? -1 : 0) +
                    (keyboardState.IsAnyKeyDown(Keys.Right, Keys.D) ? 1 : 0),
                Y = (keyboardState.IsAnyKeyDown(Keys.Up, Keys.W) ? -1 : 0) +
                    (keyboardState.IsAnyKeyDown(Keys.Down, Keys.S) ? 1 : 0),
            };

            var seed = new Random(gameTime.TotalGameTime.Seconds);
            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            entities
                .AllInteractions()
                .DoIf<IPlayer, IRoom>((P, R) =>
                {
                    var (translation, angle) = R.UpdateRoomSize(P.Avatar.Pos);
                    
                    if(direction.Y != 0)
                    {
                        if (direction.Y < 0)
                        {
                            direction.Y = 0;
                            angle *= -1;
                            direction += angle;
                        }
                        else
                        {
                            direction.Y = 0;
                            direction += angle;
                        }
                    }

                    P.Avatar.Pos += translation;
                })
                .Enumerate();

            entities
                .DoIf<IPlayer>(x =>
                    x.Avatar.Direction = direction)
                .DoIf<ICamera>(x =>
                    x.Transform = Matrix.CreateTranslation(new Vector3(-x.Target.Pos, 0.0f)))
                .DoIf<ICircle>(x =>
                    x.Pos += x.Direction * x.Speed * time)
                .Enumerate();

            var delayedActions = new List<Action>();

            entities
                .AllInteractions()
                .DoIf<ICircle, ITrigger>((C, T) =>
                {
                    var circleBoundingRect = new RectangleF(C.Pos - Vector2.One * C.Radius, Vector2.One * C.Radius * 2.0f);
                    if (circleBoundingRect.Intersects(T.Area))
                    {
                        if (!T.Safety)
                        {
                            delayedActions.Add(T.Action);
                            T.Safety = true;
                        }
                    } else if (!T.TriggerOnce)
                    {
                        T.Safety = false;
                    }
                })
                .DoIf<IPolygon, ICircle>((P, C) =>
                {
                    // TODO: Intersection utility with enums for all the cases?
                    P.Vertices
                        .ToLineSegments()
                        .Do((A, B) =>
                        {
                            A -= C.Pos;
                            B -= C.Pos;
                            var D = B - A;

                            var a = Vector2.Dot(D, D);
                            var b = 2.0f * Vector2.Dot(A, D);
                            var c = Vector2.Dot(A, A) - C.Radius * C.Radius;

                            var det = b * b - 4 * a * c;

                            if (det > 0.0f)
                            {
                                var t = (-b / a) / 2.0f;

                                var r2 = C.Radius * C.Radius;
                                var absA = Vector2.Dot(A, A);
                                var absB = Vector2.Dot(B, B);

                                if ((0.0f <= t && t <= 1.0f) || absA <= r2 || absB <= r2)
                                {
                                    var P = 0.0f <= t && t <= 1.0f
                                        ? A + D * t
                                        : 0.0f <= t
                                        ? B
                                        : A;

                                    var d = C.Radius - MathF.Sqrt(Vector2.Dot(P, P));
                                    var N = Vector2.Normalize(P == Vector2.Zero ? Vector2.UnitY : P);

                                    C.Pos -= N * d;
                                }
                            }
                        })
                        .Enumerate();
                })
                .Enumerate();

            delayedActions
                .ForEach(action => action());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var camera = entities.Where<ICamera>().Single();

            spriteBatch.Begin(transformMatrix: cameraOffset * camera.Transform);

            entities
                .DoIf<IPolygon>(x =>
                    // TODO: Draw linesegment extension?
                    //       https://github.com/craftworkgames/MonoGame.Extended/blob/develop/src/cs/MonoGame.Extended/VectorDraw/PrimitiveDrawing.cs ?
                    x.Vertices
                        .ToLineSegments()
                        .Do((a, b) =>
                            spriteBatch.Draw(
                                textureDict["line"],
                                new Rectangle(a.ToPoint(), (Vector2.UnitX * Vector2.Distance(a, b) + Vector2.UnitY).ToPoint()),
                                null,
                                x.Color,
                                (b - a).ToAngle(),
                                Vector2.Zero,
                                SpriteEffects.None,
                                0.0f))
                        .Enumerate())
                .DoIf<ISprite>(x =>
                    spriteBatch.Draw(textureDict[x.TextureName], x.Pos - textureDict[x.TextureName].Bounds.Size.ToVector2() * 0.5f, x.Color))
                .DoIf<IPlayer>(x =>
                    spriteBatch.Draw(
                        textureDict["line"],
                        new Rectangle(x.Avatar.Pos.ToPoint(), (new Vector2(100.0f, 1.0f)).ToPoint()),
                        null,
                        Color.Red,
                        Vector2.Normalize(x.Avatar.Direction).ToAngle(),
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.0f))
                .DoIf<IText>(x =>
                    spriteBatch.DrawString(fontDict[x.SpriteFont], x.Text, x.Pos, x.Color))
                .DoIf<ITrigger>(x =>
                    spriteBatch.Draw(textureDict["line"], x.Area.ToRectangle(), null, new Color(x.Safety ? Color.Yellow : Color.Red, 0.0f)))
                .Enumerate();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}