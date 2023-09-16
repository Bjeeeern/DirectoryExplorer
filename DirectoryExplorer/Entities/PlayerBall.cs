using DirectoryExplorer.Primitives;
using Color = Microsoft.Xna.Framework.Color;

namespace DirectoryExplorer.Entities
{
    class PlayerBall : Ball, IPlayer
    {
        new public Color Color { get; set; } = Color.Yellow;
    }
}