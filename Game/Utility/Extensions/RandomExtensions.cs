using Microsoft.Xna.Framework;
using System;

namespace DirectoryExplorer.Utility.Extensions
{
    static class RandomExtensions
    {
        public static Vector2 NextUnitSquareVector2(this Random random)
        {
            return new Vector2(random.NextSingle(), random.NextSingle()) * 2.0f - new Vector2(1.0f);
        }

        public static Vector2 NextUnitVector2(this Random random)
        {
            var v = random.NextUnitSquareVector2();
            v.Normalize();
            return v;
        }
    }
}
