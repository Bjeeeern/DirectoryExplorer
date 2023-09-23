using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using DirectoryExplorer.Utility;
using DirectoryExplorer.Utility.Extensions;
using DirectoryExplorer.Services.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DirectoryExplorer
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphicsDeviceManager;

        private List<IEntity> entities;
        private Dictionary<string, Texture2D> textureDict;
        private Dictionary<string, SpriteFont> fontDict;
        private Matrix cameraOffset;

        public Game()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphicsDeviceManager.SetScreenScale(1920, 1080);
            graphicsDeviceManager.ApplyChanges();

            cameraOffset = Matrix.CreateWorld(graphicsDeviceManager.GetScreenScale() * 0.5f, Vector3.Forward, Vector3.Up);

            Services.AddService<IDirectoryExplorer>(new Services.Providers.DirectoryExplorer());

            var dirExplorer = Services.GetService<IDirectoryExplorer>();

            entities = new Builder()
                .BeginCamera(pop => pop
                    .Add<Ball>())
                .BeginCamera(pop => pop
                    .Add<Player>()
                    .Concat(dirExplorer.BuildEntitiesFromPath(".")))
                .ToList();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textureDict = entities
                .Where<ISprite>()
                .DistinctBy(x => x.TextureName)
                .ToDictionary(x => x.TextureName, x => Content.Load<Texture2D>($"images/{x.TextureName}"));

            textureDict["line"] = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            textureDict["line"].SetData(new[] { Color.White }, 0, 1);

            fontDict = new() {
                { "default", Content.Load<SpriteFont>("fonts/default") }
            };
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

            entities
                .Where<IPlayer>()
                .Do(x => x.Camera!.Direction = direction)
                .Enumerate();

            var seed = new Random(gameTime.TotalGameTime.Seconds);
            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            entities
                .IfDo<ICamera>(x =>
                    x.Transform *= Matrix.CreateTranslation(new Vector3(-x.Direction, 0.0f) * x.Speed * time))
                .IfDo<IBody>(x =>
                    x.Pos += x.Direction * x.Speed * time)
                .Enumerate();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach(var camera in entities.Where<ICamera>())
            {
                spriteBatch.Begin(transformMatrix: cameraOffset * camera.Transform);

                camera.Children
                    .IfDo<IPolygon>(x =>
                    {
                        var verts = x.Vertices.ToArray();

                        for (int i = 0; i < verts.Length; i++)
                        {
                            var n1 = verts[i];
                            var n2 = verts[(i + 1) % verts.Length];

                            spriteBatch.Draw(
                                textureDict["line"],
                                new Rectangle(n1.ToPoint(), (Vector2.UnitX * Vector2.Distance(n1, n2) + Vector2.UnitY).ToPoint()),
                                null,
                                x.Color,
                                (n2 - n1).ToAngle(),
                                Vector2.Zero,
                                SpriteEffects.None,
                                0.0f);
                        }
                    })
                    .IfDo<ISprite>(x => spriteBatch.Draw(textureDict[x.TextureName], x.Pos - textureDict[x.TextureName].Bounds.Size.ToVector2() * 0.5f, x.Color))
                    .IfDo<IText>(x => spriteBatch.DrawString(fontDict[x.SpriteFont], x.Content, x.Pos, x.Color))
                    .Enumerate();

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}