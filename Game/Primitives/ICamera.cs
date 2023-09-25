using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Primitives
{
    internal interface ICamera: IEntity
    {
        Matrix Transform { get; set; }

        IPositioned Target { get; set; }
    }
}
