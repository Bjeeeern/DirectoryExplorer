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

        private List<IEntity> Entities;
        private Dictionary<string, Texture2D> TextureDict;
        private Dictionary<string, SpriteFont> FontDict;

        public Game()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphicsDeviceManager.IsFullScreen = false;
            graphicsDeviceManager.PreferredBackBufferWidth = 1920;
            graphicsDeviceManager.PreferredBackBufferHeight = 1080;
            graphicsDeviceManager.ApplyChanges();

            Services.AddService<IDirectoryExplorer>(new Services.Providers.DirectoryExplorer());

            var dirExplorer = Services.GetService<IDirectoryExplorer>();

            Entities = Builder
                .Build<Player>()
                .Concat(dirExplorer.BuildEntitiesFromPath("."))
                .ToList();

            // TODO: Cleanup, especially of camera abstraction
            var camera = Entities.Single(x => x is ICamera && x is IPlayer) as ICamera;

            // TODO: Use helper functions instead
            var t = camera.Transform;
            t.M41 = graphicsDeviceManager.PreferredBackBufferWidth * 0.5f;
            t.M42 = graphicsDeviceManager.PreferredBackBufferHeight * 0.5f;
            camera.Transform = t;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureDict = Entities
                .Only<ISprite>()
                .DistinctBy(x => x.TextureName)
                .ToDictionary(x => x.TextureName, x => Content.Load<Texture2D>($"images/{x.TextureName}"));

            TextureDict["line"] = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            TextureDict["line"].SetData(new []{ Color.White }, 0, 1);

            FontDict = new() {
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
                X = (keyboardState.IsKeyDown(Keys.Left) ? -1 : 0) +
                    (keyboardState.IsKeyDown(Keys.Right) ? 1 : 0),
                Y = (keyboardState.IsKeyDown(Keys.Up) ? -1 : 0) +
                    (keyboardState.IsKeyDown(Keys.Down) ? 1 : 0),
            };

            var seed = new Random(gameTime.TotalGameTime.Seconds);
            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Entities
                .WhereDo<ICamera>(
                    x => x is IPlayer,
                    x =>
                    {
                        // TODO: Use helper functions instead
                        var t = x.Transform;
                        t.M41 -= direction.X * 100.0f * time;
                        t.M42 -= direction.Y * 100.0f * time;
                        x.Transform = t;
                    })
                .IfDo<IBody>(x => x.Pos += x.Direction * x.Speed * time)
                .Enumerate();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            var camera = Entities.Single(x => x is ICamera && x is IPlayer) as ICamera;

            Entities
                .IfDo<IPolygon>(x => {
                    var verts = x.Vertices
                        .Select(y => Vector2.Transform(y, camera.Transform))
                        .ToArray();

                    for(int i = 0; i < verts.Length; i++) 
                    {
                        var n1 = verts[i];
                        var n2 = verts[(i + 1) % verts.Length];

                        spriteBatch.Draw(
                            TextureDict["line"],
                            new Rectangle(
                                n1.ToPoint(),
                                new Point((int)Vector2.Distance(n1, n2), 1)),
                            null,
                            x.Color,
                            (n2 - n1).ToAngle(),
                            new Vector2(0f, 0f),
                            SpriteEffects.None,
                            1.0f);
                    }
                })
                .IfDo<ISprite>(x => spriteBatch.Draw(TextureDict[x.TextureName], Vector2.Transform(x.Pos, camera.Transform), x.Color))
                .IfDo<IText>(x => spriteBatch.DrawString(FontDict[x.SpriteFont], x.Content, Vector2.Transform(x.Pos, camera.Transform), x.Color))
                .Enumerate();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}