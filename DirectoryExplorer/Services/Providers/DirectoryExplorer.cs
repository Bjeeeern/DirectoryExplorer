using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DirectoryExplorer.Services.Interfaces;
using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Services.Providers
{
    class DirectoryExplorer : IDirectoryExplorer
    {
        public IEnumerable<IEntity> BuildEntitiesFromPath(string path) =>
            Directory.Exists(path)
                ? Directory.EnumerateDirectories(path)
                    .Append(Directory.GetCurrentDirectory())
                    .Concat(Directory.EnumerateFiles(path))
                    .Select(x => new Text { Content = x })
                    .Cast<IEntity>()
                    .Append(new Room { Vertices = new[] {
                        new Vector2(-100.0f, -100.0f),
                        new Vector2(100.0f, -100.0f),
                        new Vector2(100.0f, 100.0f),
                        new Vector2(-100.0f, 100.0f),
                    }})
                : Enumerable.Empty<IEntity>();
    }
}