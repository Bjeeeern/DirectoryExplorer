namespace DirectoryExplorer.Primitives
{
    interface ICircle : IPositioned, IMovable
    {
        float Radius { get; set; }
    }
}