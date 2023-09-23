using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DirectoryExplorer.Utility.Extensions
{
    internal static class Vector2Extensions
    {
        public static float ToAngle(this Vector2 vector)
        {
            return MathF.Atan2(vector.Y, vector.X);
        }

        public static IEnumerable<(Vector2 A, Vector2 B)> ToLineSegments(this IEnumerable<Vector2> vertices) =>
            vertices.Zip(vertices.Skip(1).Append(vertices.First()));
    }
}
