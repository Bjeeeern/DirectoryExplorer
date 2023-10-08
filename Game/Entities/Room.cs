using Game.Primitives;
using Game.Utility.Extensions;
using Game.Services.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Shapes;

namespace Game.Entities
{
    internal class Room : IRoom
    {
        public Color Color { get; set; } = Color.DarkRed;
        public List<Polyline> Walls { get; set; }

        private readonly Vector2 origin;
        private readonly Vector2 singleCharSize;

        private readonly ParentDirText parentDir;
        private readonly List<FileText> files;
        private readonly List<SubDirectoryText> subDirs;

        private readonly float width;
        private readonly float height;
        private readonly Vector2 topMiddle;
        private const float padding = 20.0f;

        public Room(Vector2 origin, Vector2 singleCharSize, Directory directory, out IEnumerable<IEntity> subEntities)
        {
            this.origin = origin;
            this.singleCharSize = singleCharSize;

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

            subEntities = subDirs
                .Concat<IEntity>(files)
                .Append(parentDir);

            // TODO: more advanced width calculation
            width = 1000.0f;//600.0f + MathF.Max(0.0f, subDirs.Count - 3) * 50.0f;
            height = 200.0f + padding * 2.0f + RowDistance * FileRows;
            topMiddle = new Vector2(0, -0.5f * height) + origin;

            UpdateRoomSize();
        }

        private int FileColumns => Math.Min(1 + files.Count / 10, 3);
        private int FileRows => files.Count <= 30
                ? Math.Min(10, files.Count / FileColumns)
                : files.Count / FileColumns + files.Count % 3;
        private float RowDistance => singleCharSize.Y * 1.5f;

        public void UpdateRoomSize()
        {
            var topLeft = new Vector2(width, height) * -0.5f + origin;

            parentDir.Pos = topLeft + new Vector2(padding);

            var columnWidth = (width - padding * (FileColumns + 1)) / FileColumns;
            var charsPerColumn = columnWidth / singleCharSize.X;

            var rowDistance = RowDistance;
            var columnDistance = columnWidth + padding;

            files.Do((x, i) =>
            {
                x.Limit = (int)charsPerColumn;
                x.Pos =
                    topLeft +
                    new Vector2(padding, padding + rowDistance) +
                    new Vector2(i / FileRows, i % FileRows) *
                    new Vector2(columnDistance, rowDistance);
            }).Enumerate();

            Walls = new List<Polyline> {
                new Polyline(new []{
                    new Vector2(0.0f, 0.0f),
                    new Vector2(width, 0.0f),
                    new Vector2(width, height),
                    new Vector2(0.0f, height),
                }.Select(p => topLeft + p))
            };
        }

        private Vector2 WorldToRoom(Vector2 point)
        {
            return point - topMiddle;
        }

        public float GetHorizontalPointScalar(Vector2 point)
        {
            var pointInRoom = WorldToRoom(point);
            var t = pointInRoom.Y / height;
            return MathHelper.Lerp(1.0f, 2.0f, MathHelper.Clamp(t, 0.0f, 1.0f));
        }

        public Vector2 DoRoomTransform(Vector2 point, float scalar)
        {
            var pointInRoom = point - topMiddle;

            pointInRoom.X *= scalar;

            return pointInRoom + topMiddle;
            /*
            var topLeft = new Vector2(topWidth, height) * -0.5f + origin;
            
            var northDoorWidth = padding * 4;

            walls[0].Polyline.Points[0] = Vector2.Zero;
            walls[0].Polyline[1] = new Vector2((topWidth - northDoorWidth) * 0.5f, 0.0f);
            walls[1].Polyline[0] = new Vector2((topWidth + northDoorWidth) * 0.5f, 0.0f);
            walls[1].Polyline[1] = new Vector2(topWidth, 0.0f);

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

            walls[2].Polyline[0] = Vector2.Zero;
            walls[2].Polyline[1] = new Vector2(bottomWidthStart, height);
            walls[3].Polyline[0] = new Vector2(topWidth, 0.0f);
            walls[3].Polyline[1] = new Vector2(bottomWidthStart + bottomWidth, height);

            walls[4].Polyline[0] = new Vector2(bottomWidthStart, height);
            walls[4].Polyline[1] = new Vector2(bottomWidthStart + unallocatedWidth * 0.5f + padding, height);
            walls[5].Polyline[0] = new Vector2(bottomWidthStart + bottomWidth - unallocatedWidth * 0.5f - padding, height);
            walls[5].Polyline[1] = new Vector2(bottomWidthStart + bottomWidth, height);

            walls.Skip(6).Do((x, i) =>
            {
                var doorEnd = bottomWidthStart + unallocatedWidth * 0.5f + padding +
                    i * southDoorDistance + southDoorWidth;
                x.Polyline[0] = new Vector2(doorEnd, height);
                x.Polyline[1] = new Vector2(doorEnd + padding, height);
            })
            .Enumerate();

            foreach (var wall in walls)
                for (var i = 0; i < wall.Polyline.Count; i++)
                    wall.Polyline[i] += topLeft;

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
            */
        }
    }
}
