namespace DirectoryExplorer.Primitives
{
    internal interface IPlayer: IEntity
    {
        ICamera? Camera { get; set; }
    }
}
