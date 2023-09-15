using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX;
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

        public Game()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Entities = new List<IEntity>
            {
                new PlayerBall(),
                new Ball(),
                new Ball(),
                new Ball(),
                new Ball(),
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Entities
                .IfDo<Ball>(x => x.Texture = Content.Load<Texture2D>("ball"))
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

            Entities
                .Where(x => x is IPlayer && x is IMovable)
                .Cast<IMovable>()
                .Single()
                .Direction = direction;

            var seed = new Random(gameTime.TotalGameTime.Seconds);
            foreach (var body in Entities
                .Where(x => x is not IPlayer && x is IMovable)
                .Cast<IMovable>())
            {
                body.Direction = new Vector2(seed.NextFloat(-1.0f, 1.0f), seed.NextFloat(-1.0f, 1.0f));
            }

            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Entities
                .IfDo<IBody>(x => x.Pos += x.Direction * x.Speed * time)
                .Enumerate();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            
            Entities
                .IfDo<ISprite>(x => spriteBatch.Draw(x.Texture, x.Pos, Color.White))
                .Enumerate();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    class PlayerBall : Ball, IPlayer
    {
    }

    class Ball : IBody, ISprite, IEntity
    {
        public Texture2D Texture { get; set; }
        public Vector2 Pos { get; set; }
        public float Speed { get; set; } = 100.0f;
        public Vector2 Direction { get; set; }
    }

    interface IPlayer
    {
    }

    interface IBody : IPositioned, IMovable
    {
    }

    internal interface ISprite : IPositioned
    {
        Texture2D Texture { get; set; }
    }

    internal interface IMovable
    {
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }
    }

    internal interface IPositioned
    {
        public Vector2 Pos { get; set; }
    }

    internal interface IEntity
    {
    }

    static class IEnumerableExtensions
    {
        public static IEnumerable<IEntity> IfDo<T>(this IEnumerable<IEntity> list, Action<T> action) where T: class =>
            list.Select(e => {
                if (e is T) action(e as T);
                return e;
            });
        public static void Enumerate(this IEnumerable<IEntity> list) => list.Count();
    }
}