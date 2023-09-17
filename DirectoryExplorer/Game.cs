using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using DirectoryExplorer.Utility;
using DirectoryExplorer.Utility.Extensions;
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

            Entities = Builder
                .Build<Ball>(6)
                .Add<Text>(6)
                .Add<PlayerBall>()
                .ToList();

            var seed = Random.Shared;
            Entities
                .IfDo<IPositioned>(x => x.Pos =
                    new Vector2(
                        graphicsDeviceManager.PreferredBackBufferWidth,
                        graphicsDeviceManager.PreferredBackBufferHeight) * 0.5f)
                .WhereDo<IPositioned>(
                    x => x is not IPlayer,
                    x => x.Pos += seed.NextUnitSquareVector2() * new Vector2(200.0f, 100.0f))
                .IfDo<IText>(x => x.Text = string.Concat(Enumerable.Range(0, seed.Next(5, 10)).Select(x => (char)seed.Next('a', 'z'))))
                .Enumerate();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureDict = Entities
                .Only<ISprite>()
                .DistinctBy(x => x.TextureName)
                .ToDictionary(x => x.TextureName, x => Content.Load<Texture2D>($"images/{x.TextureName}"));

            FontDict = new() {
                { "default", Content.Load<SpriteFont>("fonts/default") }
            };

            Entities
                .IfDo<ISprite>(x => x.Pos -= TextureDict[x.TextureName].Bounds.Size.ToVector2() * 0.5f)
                .Enumerate();
        }

        protected override void Update(GameTime gameTime)
        {
            var gamePadState = GamePad.GetState(PlayerIndex.One);
            var keyboardState = Keyboard.GetState();

            if (gamePadState.Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape))
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
                .WhereDo<IMovable>(
                    x => x is IPlayer,
                    x => x.Direction = direction)
                .WhereDo<IMovable>(
                    x => x is not IPlayer,
                    x => x.Direction = seed.NextUnitVector2())
                .IfDo<IBody>(x => x.Pos += x.Direction * x.Speed * time)
                .Enumerate();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            Entities
                .IfDo<ISprite>(x => spriteBatch.Draw(TextureDict[x.TextureName], x.Pos, x.Color))
                .IfDo<IText>(x => spriteBatch.DrawString(FontDict[x.SpriteFont], x.Text, x.Pos, x.Color))
                .Enumerate();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}