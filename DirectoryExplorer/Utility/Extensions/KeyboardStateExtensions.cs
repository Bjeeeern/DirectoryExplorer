﻿using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace DirectoryExplorer.Utility.Extensions
{
    internal static class KeyboardStateExtensions
    {
        public static bool IsAnyKeyDown(this KeyboardState state, params Keys[] keys) =>
            keys.Any(x => state.IsKeyDown(x));
    }
}
