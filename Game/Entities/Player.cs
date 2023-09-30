using Game.Primitives;
using Game.Primitives;

namespace Game.Entities
{
    class Player : IPlayer, IRoomTracker
    {
        public string CurrentDirectory { get; set; } = ".";
        public IBody Avatar { get; set; }
    }
}