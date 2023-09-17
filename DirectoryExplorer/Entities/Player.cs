using DirectoryExplorer.Primitives;
using Color = Microsoft.Xna.Framework.Color;

namespace DirectoryExplorer.Entities
{
    class Player : IPlayer
    {
        public string CurrentDirectory { get; set; } = ".";
    }
}