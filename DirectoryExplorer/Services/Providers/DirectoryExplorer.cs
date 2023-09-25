using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DirectoryExplorer.Services.Interfaces;
using Microsoft.Xna.Framework;
using System;
using File = DirectoryExplorer.Entities.File;
using DirectoryExplorer.Utility;
using DirectoryExplorer.Utility.Extensions;

namespace DirectoryExplorer.Services.Providers
{
    class DirectoryExplorer : IDirectoryExplorer
    {
        // TODO: Add trigger entities
        // TODO: Separate directory service from room builder service
        // TODO: Load DirectoryInfo, FileInfo, FileSystemInfo, DriveInfo etc. instead of strings.
        // TODO: Make file more yellow depending on file age, and more bold depending on file size.
        public IEnumerable<IEntity> BuildEntitiesFromPath(List<IEntity> entities, string path, Vector2 origin = new Vector2(), bool addNorthTrigger = true, bool addSouthTrigger = true)
        {
            if (!Directory.Exists(path)) return Enumerable.Empty<IEntity>();

            var files = Directory.EnumerateFiles(path)
                    .Select((x, i) => new File
                    {
                        Content = x.Length > 20 ? $"{x[0..15]}...{x[^6..^0]}" : x,
                        Pos = Vector2.One * 0.1f + new Vector2(0.4f, 0.05f) * new Vector2((i / 10), (i % 10))
                    })
                    .ToList();

            var fileCount = files.Count();
            var height = 20.0f * MathF.Min(fileCount, 10) + 40.0f + 500.0f;

            var subDir = Directory.EnumerateDirectories(path)
                    .Select((x, i) => new SubDirectory
                    {
                        Content = x.Length > 20 ? $"{x[0..15]}...{x[^6..^0]}" : x,
                        Pos = new Vector2(0.1f + 0.2f * i, 0.9f)
                    })
                    .ToList();

            var subDirCount = subDir.Count();

            var width = 400.0f * (fileCount / 10 + 1) + 40.0f;

            var parent = Directory.GetParent(path)?.Name ?? Directory.GetDirectoryRoot(path);
            var parentDir = new ParentDirectory
            {
                Content = parent,
                Pos = new Vector2(0.1f, 0.0f)
            };

            var scale = new Vector2(width, height);
            var entityTranslate = Vector2.One * -0.5f;
            var worldTranslate = Vector2.One * -20.0f + origin;
            var entityToWorld = (Vector2 x) => (x + entityTranslate) * scale + worldTranslate;

            var room = new Room
            {
                Walls =
                    new[] {
                        // TODO: Codify type of walls
                        new[] {
                            new Vector2(0.4f, 0.0f),
                            new Vector2(0.0f, 0.0f),
                            new Vector2(0.0f, 1.0f),
                            new Vector2(0.4f, 1.0f),
                        },
                        new[]
                        {
                            new Vector2(0.6f, 0.0f),
                            new Vector2(1.0f, 0.0f),
                            new Vector2(1.0f, 1.0f),
                            new Vector2(0.6f, 1.0f),
                        }
                    }
                    .Select(x => x.Concat(x.Skip(1).Reverse().Skip(1)))
                    .Select(x => new Wall { Vertices = x.Select(entityToWorld).ToList() })
                    .ToList()
            };

            Enumerable.Empty<IPositioned>()
                .Concat(files)
                .Concat(subDir)
                .Append(parentDir)
                .Do(x => x.Pos = entityToWorld(x.Pos))
                .Enumerate();

            var northTrigger = new Trigger
            {
                TriggerOnce = true,
                Area = new RectangleF(entityToWorld(new Vector2(0.4f, 0.0f)), new Vector2(0.2f, 0.1f) * scale),
                Action = () =>
                {
                    if (Directory.GetParent(path) != null)
                        entities.AddRange(
                            // TODO: Collection modified error. Queue creation of entities?
                            BuildEntitiesFromPath(entities, Directory.GetParent(path)!.FullName, origin - new Vector2(0.0f, height), addSouthTrigger: false));
                }
            };

            var southTrigger = new Trigger
            {
                TriggerOnce = true,
                Area = new RectangleF(entityToWorld(new Vector2(0.4f, 0.9f)), new Vector2(0.2f, 0.1f) * scale),
                Action = () =>
                {
                    var first = new DirectoryInfo(path).EnumerateDirectories().FirstOrDefault();
                    if (first != null)
                        entities.AddRange(
                            BuildEntitiesFromPath(entities, first.FullName, origin + new Vector2(0.0f, height), addNorthTrigger: false));
                }
            };

            var result = subDir
                .Cast<IEntity>()
                .Append(parentDir)
                .Concat(subDir)
                .Concat(files)
                .Append(room)
                .Concat(room.Walls);

            // TODO: Utility functions for this. AppendIf/
            if (addNorthTrigger) result = result.Append(northTrigger);
            if (addSouthTrigger) result = result.Append(southTrigger);

            return result;
        }
    }
}