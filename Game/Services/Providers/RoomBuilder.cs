using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using DirectoryExplorer.Services.Interfaces;
using DirectoryExplorer.Utility;
using DirectoryExplorer.Utility.Extensions;
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
            var singleCharSize = defaultFont.MeasureString("O");
            var room = new Room(origin, width, singleCharSize, directory);

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
                .Append(room.parentDir)
                .Concat(room.files)
                .Concat(room.subDirs)
                .Concat(room.walls)
                .Append(room);

            // TODO: Utility functions for this. AppendIf
            // if (directory.Parent != null) result = result.Append(northTrigger);
            // if (directory.Children.Any()) result = result.Append(southTrigger);

            return result.ToList();
        }
    }
}
