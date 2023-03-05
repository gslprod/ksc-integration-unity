namespace Utilities
{
    public static class StringUtilities
    {
        public static string NameFormat(this string source)
            => string.IsNullOrEmpty(source) ? "Безымянный объект" : source;

        public static string Clamp(this string source, int maxLength)
            => source.Length > maxLength ? source.Substring(0, maxLength - 4) + "..." : source;

        public static string RemoveHostFromIPAddress(this string source)
            => source.Split(':')[0];

        public static bool ContainsIn(this string source, bool caseSensitive, params string[] inStrings)
        {
            foreach (var inString in inStrings)
            {
                var inStringToCheck = caseSensitive ? inString : inString.ToLower();
                var sourceToCheck = caseSensitive ? source : source.ToLower();

                if (inStringToCheck.Contains(sourceToCheck))
                    return true;
            }

            return false;
        }
    }
}