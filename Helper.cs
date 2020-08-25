

using System.Collections.Generic;

namespace MsBuildDebugger
{
    public static class Helper
    {
        /// <summary>
        /// Split a list literal into its items and remove any whitespace surrounding the items.
        /// </summary>
        public static IEnumerable<string> SplitValue(string val)
        {
            val = val.Replace("\r\n", ";");
            var split = val.Split(';');
            var result = new List<string>();
            for(var i = 0; i < split.Length; i++)
            {
                var trimmed = split[i].Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    result.Add(trimmed);
                }
            }
            return result;
        }

        public static string AlignStringInSpace(string input, int space, HorizontalAlignment alignment)
        {
            var result = input;
            if (input.Length > space)
            {
                result = input.Substring(0, space - 3) + "...";
            }
            return result;
        }
    }

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }
}
