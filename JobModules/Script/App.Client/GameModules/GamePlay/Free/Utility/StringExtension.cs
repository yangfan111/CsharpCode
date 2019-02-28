using System;

namespace Assets.Sources.Free.Utility
{
    public static class StringExtension
    {
        public static string[] Split(this string str, string seperator)
        {
            return str.Split(new[] {seperator}, StringSplitOptions.None);
        }
    }
}
