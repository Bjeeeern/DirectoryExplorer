using Game.Primitives;
using Game.Utility.Extensions;
using Game.Services.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Entities
{
    internal class Room : IRoom
    {
        public List<IPolygon> walls { get; set; }

        public ParentDirText parentDir;
        public List<FileText> files;
        public List<SubDirectoryText> subDirs;
        private readonly Vector2 origin;
        private readonly float topWidth;
        private readonly Vector2 singleCharSize;

        public Room(Vector2 origin, float topWidth, Vector2 singleCharSize, Directory directory)
        {
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
                .Select((x, i) => new FileText { Content = x.Name })
                .ToList();
            subDirs = directory.Children
                .Select((x, i) => new SubDirectoryText { Content = x.Name })
                .ToList();

            this.origin = origin;
            this.topWidth = topWidth;
            this.singleCharSize = singleCharSize;

            UpdateRoomSize(origin);
        }

        // TODO: make not static
        public static float prevDX;
        public static float prevDY;

        public (Vector2 Translation, Vector2 Angle) UpdateRoomSize(Vector2 playerPos)
        {
            var padding = 20.0f;

            var columns = Math.Min(1 + files.Count / 10, 3);
            var columnWidth = (topWidth - padding * (columns + 1)) / columns;

            var columnDistance = columnWidth + padding;
            var rowDistance = singleCharSize.Y * 1.5f;

            var rows = files.Count <= 30
                ? Math.Min(10, files.Count / columns)
                : files.Count / columns + files.Count % 3;

            var height = (topWidth * 0.5f) + padding * 2.0f + rowDistance * rows;
            var topLeft = new Vector2(topWidth, height) * -0.5f + origin;

            var northDoorWidth = padding * 4;

            walls[0].Vertices[0] = Vector2.Zero;
            walls[0].Vertices[1] = new Vector2((topWidth - northDoorWidth) * 0.5f, 0.0f);
            walls[1].Vertices[0] = new Vector2((topWidth + northDoorWidth) * 0.5f, 0.0f);
            walls[1].Vertices[1] = new Vector2(topWidth, 0.0f);

            var neededWidth = northDoorWidth * subDirs.Count + padding * (subDirs.Count + 1);

            var t = MathHelper.Clamp((playerPos - origin).Y / (height * 0.5f - padding * 4.0f), 0.0f, 1.0f);
            var bottomWidth = MathHelper.Lerp(topWidth, neededWidth, t);
            var bottomWidthStart = (bottomWidth - topWidth) * -0.5f;

            var dY = MathHelper.Clamp((playerPos.Y - topLeft.Y) / height, 0.0f, 1.0f);
            var toWall = MathHelper.Lerp(topWidth, bottomWidth, dY) * 0.5f;

            var dX = MathHelper.Clamp((playerPos.X - origin.X) / toWall, -1.0f, 1.0f);

            var ddY = dY - prevDY;
            prevDY = dY;

            var ddX = dX - prevDX;
            prevDX = dX;

            var playerTranslation = ddY != 0 ? new Vector2(-ddX * toWall, 0.0f) : Vector2.Zero;

            var goal = new Vector2(dX * bottomWidth * 0.5f, topLeft.Y + height);
            var playerAngle = Vector2.Normalize(goal - playerPos);

            var allocatableWidth = bottomWidth - padding * (subDirs.Count + 1);
            var southDoorWidth = MathF.Min(northDoorWidth, allocatableWidth / subDirs.Count);

            var southDoorDistance = southDoorWidth + padding;
            var unallocatedWidth = allocatableWidth - (southDoorWidth * subDirs.Count);

            walls[2].Vertices[0] = Vector2.Zero;
            walls[2].Vertices[1] = new Vector2(bottomWidthStart, height);
            walls[3].Vertices[0] = new Vector2(topWidth, 0.0f);
            walls[3].Vertices[1] = new Vector2(bottomWidthStart + bottomWidth, height);

            walls[4].Vertices[0] = new Vector2(bottomWidthStart, height);
            walls[4].Vertices[1] = new Vector2(bottomWidthStart + unallocatedWidth * 0.5f + padding, height);
            walls[5].Vertices[0] = new Vector2(bottomWidthStart + bottomWidth - unallocatedWidth * 0.5f - padding, height);
            walls[5].Vertices[1] = new Vector2(bottomWidthStart + bottomWidth, height);

            walls.Skip(6).Do((x, i) => {
                var doorEnd = bottomWidthStart + unallocatedWidth * 0.5f + padding +
                    i * southDoorDistance + southDoorWidth;
                x.Vertices[0] = new Vector2(doorEnd, height);
                x.Vertices[1] = new Vector2(doorEnd + padding, height);
            })
            .Enumerate();

            foreach (var wall in walls)
                for (var i = 0; i < wall.Vertices.Count; i++)
                    wall.Vertices[i] += topLeft;

            parentDir.Pos = topLeft + new Vector2(padding);

            var charsPerColumn = columnWidth / singleCharSize.X;

            files.Do((x, i) =>
            {
                x.Limit = (int)charsPerColumn;
                x.Pos =
                    topLeft +
                    new Vector2(padding, padding + rowDistance) +
                    new Vector2(i / rows, i % rows) *
                    new Vector2(columnDistance, rowDistance);
            }).Enumerate();

            var charsPerDoor = southDoorWidth / singleCharSize.X;

            subDirs.Do((x, i) =>
            {
                x.Limit = (int)charsPerDoor;
                x.Pos =
                    topLeft +
                    new Vector2(
                        bottomWidthStart + unallocatedWidth * 0.5f + padding + i * southDoorDistance,
                        height - (padding + singleCharSize.Y));
            }).Enumerate();

            return (playerTranslation, playerAngle);
        }
    }
}
