using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Entities
{
    class Player : IPlayer, ICamera, IRoomTracker
    {
        public string CurrentDirectory { get; set; } = ".";

        public Matrix Transform { get; set; } = Matrix.Identity;
    }
}