using DirectoryExplorer.Primitives;
using DirectoryExplorer.Utility.Extensions;
using Game.Services.Models;
using Game.Utility.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DirectoryExplorer.Entities
{
    internal class Room : IRoom
    {
        public List<IPolygon> walls { get; set; }

        public ParentDirText parentDir;
        public List<FileText> files;
        public List<SubDirectoryText> subDirs;

        public Room(Vector2 origin, float minWidth, Vector2 singleCharSize, Directory directory)
        {
            var padding = 20.0f;

            var columns = Math.Min(1 + directory.Files.Count / 10, 3);
            var columnWidth = (minWidth - padding * (columns + 1)) / columns;

            var charsPerColumn = columnWidth / singleCharSize.X;

            var northDoorWidth = padding * 4;

            var allocatableWidth = minWidth - padding * (directory.Children.Count + 1);
            var southDoorWidth = MathF.Min(northDoorWidth, allocatableWidth / directory.Children.Count);

            var charsPerDoor = southDoorWidth / singleCharSize.X;

            walls = Enumerable.Range(0, 6 + directory.Children.Count - 1)
                .Select(_ => new Wall
                {
                    Vertices = new[] { Vector2.Zero, Vector2.Zero }
                })
                .Cast<IPolygon>()
                .ToList();
            parentDir = new ParentDirText
            {
                Content = directory.Parent?.Name ?? directory.Drive.Name,
            };
            files = directory.Files
                .Select((x, i) => new FileText
                {
                    Content = x.Name.Shorten((int)charsPerColumn),
                })
                .ToList();
            subDirs = directory.Children
                .Select((x, i) => new SubDirectoryText
                {
                    Content = x.Name.Shorten((int)charsPerDoor),
                })
                .ToList();

            var columnDistance = columnWidth + padding;
            var rowDistance = singleCharSize.Y * 1.5f;

            var rows = directory.Files.Count <= 30
                ? Math.Min(10, directory.Files.Count / columns)
                : directory.Files.Count / columns + directory.Files.Count % 3;

            var height = (minWidth * 0.5f) + padding * 2.0f + rowDistance * rows;
            var topLeft = new Vector2(minWidth, height) * -0.5f + origin;

            var southDoorDistance = southDoorWidth + padding;
            var unallocatedWidth = allocatableWidth - (southDoorWidth * directory.Children.Count);

            walls[0].Vertices[0] = Vector2.Zero;
            walls[0].Vertices[1] = new Vector2((minWidth - northDoorWidth) * 0.5f, 0.0f);
            walls[1].Vertices[0] = new Vector2((minWidth + northDoorWidth) * 0.5f, 0.0f);
            walls[1].Vertices[1] = new Vector2(minWidth, 0.0f);
            walls[2].Vertices[0] = Vector2.Zero;
            walls[2].Vertices[1] = new Vector2(0.0f, height);
            walls[3].Vertices[0] = new Vector2(minWidth, 0.0f);
            walls[3].Vertices[1] = new Vector2(minWidth, height);
            walls[4].Vertices[0] = new Vector2(0, height);
            walls[4].Vertices[1] = new Vector2(unallocatedWidth * 0.5f + padding, height);
            walls[5].Vertices[0] = new Vector2(minWidth - unallocatedWidth * 0.5f - padding, height);
            walls[5].Vertices[1] = new Vector2(minWidth, height);

            walls.Skip(6).Do((x, i) => {
                var doorEnd = unallocatedWidth * 0.5f + padding +
                    i * southDoorDistance + southDoorWidth;
                x.Vertices[0] = new Vector2(doorEnd, height);
                x.Vertices[1] = new Vector2(doorEnd + padding, height);
            })
            .Enumerate();

            foreach (var wall in walls)
                for (var i = 0; i < wall.Vertices.Count; i++)
                    wall.Vertices[i] += topLeft;

            parentDir.Pos = topLeft + new Vector2(padding);

            files.Do((x, i) => x.Pos =
                topLeft +
                new Vector2(padding, padding + rowDistance) +
                new Vector2(i / rows, i % rows) *
                new Vector2(columnDistance, rowDistance)
            ).Enumerate();

            subDirs.Do((x, i) => x.Pos =
                topLeft +
                new Vector2(
                    unallocatedWidth * 0.5f + padding + i * southDoorDistance,
                    height - (padding + singleCharSize.Y))
            ).Enumerate();
        }
    }
}
