using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DirectoryExplorer.Entities
{
    internal class Camera : ICamera
    {
        public Matrix Transform { get; set; } = Matrix.Identity;
        public float Speed { get; set; } = 300.0f;
        public Vector2 Direction { get; set; } = Vector2.Zero;
        public IList<IEntity> Children { get; set; } = new List<IEntity>();
    }
}
