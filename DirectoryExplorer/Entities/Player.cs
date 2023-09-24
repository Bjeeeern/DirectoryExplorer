using DirectoryExplorer.Primitives;

namespace DirectoryExplorer.Entities
{
    class Player : IPlayer, IRoomTracker
    {
        public string CurrentDirectory { get; set; } = ".";
        public IMovable Target { get; set; }
    }
}