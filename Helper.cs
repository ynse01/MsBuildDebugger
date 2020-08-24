

namespace MsBuildDebugger
{
    public static class Helper
    {
        /// <summary>
        /// Split a list literal into its items and remove any whitespace surrounding the items.
        /// </summary>
        public static string[] SplitValue(string val)
        {
            val = val.Replace("\r\n", ";");
            var split = val.Split(';');
            for(var i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Trim();
            }
            return split;
        }
    }
}
