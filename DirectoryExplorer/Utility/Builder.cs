using System;
using System.Collections.Generic;
using System.Linq;
using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using DirectoryExplorer.Utility.Extensions;

namespace DirectoryExplorer.Utility
{
    internal class Builder
    {
        IEnumerable<IEntity> entities = Enumerable.Empty<IEntity>();

        public Builder BeginCamera(Func<IEnumerable<IEntity>, IEnumerable<IEntity>> populator)
        {
            var camera = new Camera();
            camera.Children = populator(Enumerable.Empty<IEntity>()).ToList();

            var player = camera.Children.Where<IPlayer>().FirstOrDefault();
            if (player != null) player.Camera = camera;

            entities = entities.Append(camera).Concat(camera.Children);

            return this;
        }

        public List<IEntity> ToList() => entities.ToList();
    }
}