using Game.Primitives;
using System.Collections.Generic;

namespace Game
{
    internal interface IWorldBuilder
    {
        List<IEntity> BuildWorld();
    }
}