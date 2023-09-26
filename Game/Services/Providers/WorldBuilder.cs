using DirectoryExplorer.Entities;
using DirectoryExplorer.Primitives;
using DirectoryExplorer.Utility.Extensions;
using Game.Services.Providers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DirectoryExplorer
{
    internal class WorldBuilder : IWorldBuilder
    {
        private readonly RoomBuilder roomBuilder;
        List<IEntity> entities = new List<IEntity>();

        public WorldBuilder(RoomBuilder roomBuilder)
        {
            this.roomBuilder = roomBuilder;
        }

        public List<IEntity> BuildWorld()
        {
            entities
                .Add<Camera>()
                .Add<Player>()
                .Add<Ball>()
                .AddRange(roomBuilder.BuildRoom(".", Vector2.Zero));

            var camera = entities.Where<Camera>().Single();
            var player = entities.Where<Player>().Single();
            var ball = entities.Where<Ball>().Single();

            camera.Target = ball;
            player.Target = ball;

            return entities;
        }
    }
}