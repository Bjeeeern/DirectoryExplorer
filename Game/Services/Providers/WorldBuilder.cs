using Game.Entities;
using Game.Primitives;
using Game.Utility.Extensions;
using Game.Services.Providers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Game
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
                .AddRange(roomBuilder.BuildRoom("./test", Vector2.Zero));

            var camera = entities.Where<Camera>().Single();
            var player = entities.Where<Player>().Single();
            var ball = entities.Where<Ball>().Single();

            camera.Target = ball;
            player.Avatar = ball;

            return entities;
        }
    }
}