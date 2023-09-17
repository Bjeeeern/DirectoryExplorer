using Microsoft.Xna.Framework;
using System;

namespace DirectoryExplorer.Utility.Extensions
{
    internal static class Vector2Extensions
    {
        public static float ToAngle(this Vector2 vector)
        {
            return MathF.Atan2(vector.Y, vector.X);
        }
    }
}
