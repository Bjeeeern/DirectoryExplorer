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
using MonoGame.Extended.VectorDraw;

namespace Game
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private SpriteBatch spriteBatch;
        private PrimitiveBatch primitiveBatch;
        private GraphicsDeviceManager graphicsDeviceManager;
        private ServiceProvider serviceProvider;

        private List<IEntity> entities = new ();
        private Dictionary<string, Texture2D> textureDict = new ();
        private Dictionary<string, SpriteFont> fontDict = new ();
        private Matrix worldToSpriteBatchScreen;
        private Matrix worldToPrimitiveBatchScreen;

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

            var screen = graphicsDeviceManager.GetScreenScale();
            worldToSpriteBatchScreen = Matrix.CreateWorld(screen * 0.5f, Vector3.Forward, Vector3.Up);
            worldToPrimitiveBatchScreen = Matrix.CreateOrthographic(screen.X, screen.Y, -1, 1) * Matrix.CreateReflection(new Plane(Vector3.Zero, Vector3.Up));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            primitiveBatch = new PrimitiveBatch(GraphicsDevice);

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

            if(direction.X != 0.0f && direction.Y != 0.0f) direction.Normalize();

            var seed = new Random(gameTime.TotalGameTime.Seconds);
            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            entities
                .AllInteractions()
                .DoIf<IPlayer, IRoom>((P, R) =>
                {
                    var scalar = R.GetHorizontalPointScalar(P.Avatar.Pos);

                    direction.X /= scalar;
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
                .DoIf<IRoom, ICircle>((R, C) =>
                {
                    // TODO: Intersection utility with enums for all the cases?
                    R.Walls
                        .SelectMany(w => w.Points.ToLineSegments())
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
            var room = entities.Where<IRoom>().Single();
            var avatar = entities.Where<IPlayer>().Single().Avatar;
            var scalar = room.GetHorizontalPointScalar(avatar.Pos);

            spriteBatch.Begin(transformMatrix: worldToSpriteBatchScreen * camera.Transform);

            entities
                .DoIf<ISprite>(x =>
                {
                    var pos = room.DoRoomTransform(x.Pos, scalar) - textureDict[x.TextureName].Bounds.Size.ToVector2() * 0.5f;
                    spriteBatch.Draw(textureDict[x.TextureName], pos, x.Color);
                })
                .DoIf<IText>(x =>
                    spriteBatch.DrawString(fontDict[x.SpriteFont], x.Text, x.Pos, x.Color))
                .Enumerate();

            spriteBatch.End();

            var projection = Matrix.Identity;
            var view = camera.Transform * worldToPrimitiveBatchScreen;

            primitiveBatch.Begin(ref projection, ref view);
            var primitiveDrawing = new PrimitiveDrawing(primitiveBatch);

            entities
                .DoIf<IRoom>(x => x.Walls
                    .Select(w => w.Points)
                    .Do(ps => primitiveDrawing.DrawPolygon(Vector2.Zero, ps.Select(p => room.DoRoomTransform(p, scalar)).ToArray(), x.Color))
                    .Enumerate())
                .DoIf<IPlayer>(x =>
                {
                    var pos = room.DoRoomTransform(x.Avatar.Pos, scalar);
                    primitiveDrawing.DrawSegment(pos, pos + x.Avatar.Direction * 100.0f, Color.Red);
                })
                .DoIf<ITrigger>(x =>
                    primitiveDrawing.DrawSolidRectangle(new Vector2(x.Area.X, x.Area.Y), x.Area.Width, x.Area.Height, new Color(x.Safety ? Color.Yellow : Color.Red, 0.5f)))
                .Enumerate();

            primitiveBatch.End();

            base.Draw(gameTime);
        }
    }
}