using Game.Primitives;

namespace Game.Primitives
{
    internal interface IPlayer : IEntity
    {
        IBody Avatar { get; set; }
    }
}
