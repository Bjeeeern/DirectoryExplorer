﻿namespace Game.Utility.Extensions
{
    internal static class StringExtensions
    {
        public static string Shorten(this string str, int limit)
        {
            if (str.Length <= limit) return str;

            var beginning = limit / 2;

            if (limit < 5 || beginning < 3) return str[0..limit];

            var end = beginning - 3;

            return $"{str[0..beginning]}...{str[^end..^0]}";
        }
    }
}
