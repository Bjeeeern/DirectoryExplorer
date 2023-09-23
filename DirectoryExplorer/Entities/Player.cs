using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Entities
{
    class Player : IPlayer, IRoomTracker
    {
        public string CurrentDirectory { get; set; } = ".";
        public ICamera? Camera { get; set; }
    }
}