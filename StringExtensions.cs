using System.Linq;
using System.Net;

namespace DevoirFouqueville
{

    public static class StringExtensions
    {

        public static string ToTitleCase(this string value) {

            if (string.IsNullOrWhiteSpace(value)) {
                return value;
            }

            return $"{value.First().ToString().ToUpper()}{value.Substring(1).ToLower()}";
        }

        public static string ClearAndDecode(this string value) => WebUtility.HtmlDecode(value.Replace("&nbsp;", "").ToLower().Trim());


        public static string[] FormatTask(this string[] value) {
            value[value.Length - 1] = value.Length > 2 ? string.Join(':',value.Skip(1)).Trim() : value.Last().Trim();
            return value;
        }

    }
}