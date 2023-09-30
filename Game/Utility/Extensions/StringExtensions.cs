namespace Game.Utility.Extensions
{
    internal static class StringExtensions
    {
        public static string Shorten(this string str, int limit)
        {
            if (str.Length <= limit) return str;

            if (str.Length < 5 || limit < 5) return str[0..limit];

            var beginning = limit / 2;
            var end = beginning - 3;

            return $"{str[0..beginning]}...{str[^end..^0]}";
        }
    }
}
