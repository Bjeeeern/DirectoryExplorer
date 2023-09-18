using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DirectoryExplorer.Services.Interfaces;
using Microsoft.Xna.Framework;
using System;
using File = DirectoryExplorer.Entities.File;

namespace DirectoryExplorer.Services.Providers
{
    class DirectoryExplorer : IDirectoryExplorer
    {
        public IEnumerable<IEntity> BuildEntitiesFromPath(string path)
        {
            if (!Directory.Exists(path)) return Enumerable.Empty<IEntity>();

            var files = Directory.EnumerateFiles(path)
                    .Select((x, i) => new File
                    {
                        Content = x.Length > 20 ? $"{x[0..15]}...{x[^6..^0]}" : x,
                        Pos = new Vector2(400.0f * (i / 10), 20.0f * (i % 10))
                    })
                    .ToList();

            var fileCount = files.Count();
            var height = 20.0f * MathF.Min(fileCount, 10) + 40.0f + 500.0f;

            var subDir = Directory.EnumerateDirectories(path)
                    .Select((x, i) => new File
                    {
                        Content = x.Length > 20 ? $"{x[0..15]}...{x[^6..^0]}" : x,
                        Pos = new Vector2(20.0f * i, height)
                    })
                    .ToList();

            var subDirCount = subDir.Count();

            var width = 400.0f * (fileCount / 10 + 1) + 40.0f;

            var parent = Directory.GetParent(path).FullName;
            var parentDir = new ParentDirectory
            {
                Content = parent,
                Pos = new Vector2(width * 0.5f - 200.0f, -20.0f)
            };

            var scale = new Vector2(width, height);
            var translate = new Vector2(-20.0f, -20.0f);

            return subDir
                .Cast<IEntity>()
                .Append(parentDir)
                .Concat(subDir)
                .Concat(files)
                .Append(new Room
                {
                    Vertices = new[] {
                        new Vector2(0.0f, 0.0f),
                        new Vector2(1.0f, 0.0f),
                        new Vector2(1.0f, 1.0f),
                        new Vector2(0.0f, 1.0f),
                    }.Select(x => x * scale + translate)
                });
        }
    }
}