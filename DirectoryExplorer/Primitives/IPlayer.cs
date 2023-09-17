namespace DirectoryExplorer.Primitives
{
    interface IPlayer : IEntity
    {
        string CurrentDirectory { get; }
    }
}