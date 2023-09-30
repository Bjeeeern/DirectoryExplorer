using Microsoft.Xna.Framework;

namespace Game.Utility
{
    internal struct RectangleF
    {
        public float X;

        public float Y;

        public float Width;

        public float Height;

        public float Left => X;

        public float Right => X + Width;

        public float Top => Y;

        public float Bottom => Y + Height;

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(Vector2 upperLeft, Vector2 scale)
        {
            X = upperLeft.X;
            Y = upperLeft.Y;
            Width = scale.X;
            Height = scale.Y;
        }

        public Rectangle ToRectangle() => new Rectangle((int)X, (int)Y, (int)Width, (int)Height);

        public bool Intersects(RectangleF value)
        {
            if (value.Left < Right && Left < value.Right && value.Top < Bottom)
            {
                return Top < value.Bottom;
            }

            return false;
        }
    }
}
