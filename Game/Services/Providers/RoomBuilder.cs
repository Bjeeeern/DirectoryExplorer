using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using DirectoryExplorer.Services.Interfaces;
using DirectoryExplorer.Utility;
using Game.Services.Models;
using Game.Utility.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Services.Providers
{
    // TODO: Make file more yellow depending on file age, and more bold depending on file size.
    internal class RoomBuilder
    {
        private readonly IDirectoryReader directoryReader;
        private readonly SpriteFont defaultFont;
        private Directory? directory;

        public RoomBuilder(IDirectoryReader directoryReader, SpriteFont defaultFont)
        {
            this.directoryReader = directoryReader;
            this.defaultFont = defaultFont;
        }

        public List<IEntity> BuildRoom(string path, Vector2 origin)
        {
            directory = directoryReader.ReadDirectory(path);

            var width = 1000.0f;
            var padding = 20.0f;

            var singleCharSize = defaultFont.MeasureString("O");

            var columns = Math.Min(1 + directory.Files.Count / 10, 3);
            var rows = directory.Files.Count <= 30
                ? Math.Min(10, directory.Files.Count / columns)
                : directory.Files.Count / columns + directory.Files.Count % 3;

            var columnWidth = (width - padding * (2 + columns - 1)) / columns;
            var columnDistance = columnWidth + padding;
            var charsPerColumn = columnWidth / singleCharSize.X;

            var rowDistance = singleCharSize.Y * 1.5f;
            var height = (width * 0.5f) + padding * 2.0f + rowDistance * rows;

            var scale = new Vector2(width, height);
            var topLeft = scale * -0.5f + origin;

            var northDoorWidth = padding * 4;

            var subDirs = directory.Children.Count;
            var southDoorWidth = MathF.Min(northDoorWidth, (width - padding * (2 + subDirs - 1)) / subDirs);
            var charsPerDoor = southDoorWidth / singleCharSize.X;

            var southDoorDistance = southDoorWidth + padding;
            var leftoverSpace = width - padding * (2 + subDirs - 1) - southDoorWidth * subDirs;

            var parentDir = new ParentDirText
            {
                Content = directory.Parent?.Name ?? directory.Drive.Name,
                Pos = topLeft + new Vector2(padding),
            };
            var files = directory.Files
                    .Select((x, i) => new FileText
                    {
                        Content = x.Name.Shorten((int)charsPerColumn),
                        Pos = topLeft + new Vector2(padding, padding + rowDistance) + new Vector2(i / rows, i % rows) * new Vector2(columnDistance, rowDistance),
                    });
            var subDir = directory.Children
                    .Select((x, i) => new SubDirectoryText
                    {
                        Content = x.Name.Shorten((int)charsPerDoor),
                        Pos = topLeft + new Vector2(leftoverSpace * 0.5f + i * southDoorDistance, height - (padding + singleCharSize.Y)),
                    });
            var room = new Room
            {
                Walls =
                    Enumerable.Empty<IEnumerable<Vector2>>()
                        // TODO: Codify type of walls
                        .Append(new[] {
                            new Vector2(0.0f, 0.0f),
                            new Vector2((width - northDoorWidth) * 0.5f, 0.0f),
                        })
                        .Append(new[] {
                            new Vector2((width + northDoorWidth) * 0.5f, 0.0f),
                            new Vector2(width, 0.0f),
                        })
                        .Append(new[] {
                            new Vector2(0.0f, 0.0f),
                            new Vector2(0.0f, height),
                        })
                        .Append(new[] {
                            new Vector2(width, 0.0f),
                            new Vector2(width, height),
                        })
                        .Append(new[] {
                            new Vector2(0, height),
                            new Vector2(leftoverSpace * 0.5f + padding, height),
                        })
                        .Append(new[] {
                            new Vector2(width - leftoverSpace * 0.5f - padding, height),
                            new Vector2(width, height),
                        })
                        .Concat(directory.Children.Skip(1).Select((_, i) => new[] {
                            new Vector2(leftoverSpace * 0.5f + i * southDoorDistance + southDoorWidth, height),
                            new Vector2(leftoverSpace * 0.5f + i * southDoorDistance + southDoorWidth + padding, height),
                        }))
                        .Select(x => new Wall { Vertices = x.Select(v => topLeft + v) })
                        .Cast<IPolygon>()
                        .ToList(),
            };

            /*
            var northTrigger = new Trigger
            {
                TriggerOnce = true,
                Area = new RectangleF(entityToWorld(new Vector2(0.4f, 0.0f)), new Vector2(0.2f, 0.1f) * scale),
                Action = () =>
                {
                    if (directory.Parent != null)
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
            */

            var result = Enumerable.Empty<IEntity>()
                .Append(parentDir)
                .Concat(files)
                .Concat(subDir)
                .Append(room)
                .Concat(room.Walls);

            // TODO: Utility functions for this. AppendIf
            // if (directory.Parent != null) result = result.Append(northTrigger);
            // if (directory.Children.Any()) result = result.Append(southTrigger);

            return result.ToList();
        }
    }
}
