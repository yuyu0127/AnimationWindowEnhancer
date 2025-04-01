using System;
using System.Collections.Generic;

namespace AnimationWindowEnhancer.Core
{
    public static class StringUtility
    {
        /// <summary>
        /// Gets the longest common prefix from a list of strings
        /// </summary>
        public static string GetLongestCommonPrefix(IList<string> texts)
        {
            if (texts == null || texts.Count == 0)
            {
                return "";
            }

            // Use the first string as the reference
            var prefix = texts[0];

            // Compare with other strings in the list
            for (var i = 1; i < texts.Count; i++)
            {
                while (texts[i].IndexOf(prefix, StringComparison.Ordinal) != 0)
                {
                    // If no common part is found, trim the prefix character by character
                    prefix = prefix.Substring(0, prefix.Length - 1);

                    // If no common prefix is left, return an empty string
                    if (string.IsNullOrEmpty(prefix))
                    {
                        return "";
                    }
                }
            }
            return prefix;
        }
    }
}
