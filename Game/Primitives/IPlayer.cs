namespace DirectoryExplorer.Primitives
{
    internal interface IPlayer : IEntity
    {
        IMovable Target { get; set; }
    }
}
