using Microsoft.Xna.Framework;

namespace Game.Primitives
{
    internal interface ICamera: IEntity
    {
        Matrix Transform { get; set; }

        IPositioned Target { get; set; }
    }
}
