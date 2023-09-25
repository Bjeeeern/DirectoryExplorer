using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DirectoryExplorer.Entities
{
    internal class Camera : ICamera
    {
        public Matrix Transform { get; set; } = Matrix.Identity;
        public IPositioned Target { get; set; }
    }
}
