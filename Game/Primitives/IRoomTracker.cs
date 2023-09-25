namespace DirectoryExplorer.Primitives
{
    interface IRoomTracker : IEntity
    {
        string CurrentDirectory { get; }
    }
}