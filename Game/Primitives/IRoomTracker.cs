namespace Game.Primitives
{
    interface IRoomTracker : IEntity
    {
        string CurrentDirectory { get; }
    }
}