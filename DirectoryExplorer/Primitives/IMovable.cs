using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DirectoryExplorer.Primitives
{
    internal interface IMovable
    {
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }
    }
}