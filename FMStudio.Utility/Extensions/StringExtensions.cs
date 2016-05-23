using System.Globalization;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string FormatInvariant(this string str, params object[] parameters)
        {
            return string.Format(CultureInfo.InvariantCulture, str, parameters);
        }

        public static bool Like(this string str, string pattern)
        {
            return new Regex(
                "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            ).IsMatch(str);
        }
    }
}