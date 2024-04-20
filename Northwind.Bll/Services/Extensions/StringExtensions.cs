using System.Text.RegularExpressions;

namespace Northwind.Bll.Services.Extensions
{
    public static class StringExtensions
    {
        public static string SplitForUpperCase(this string line)
        {
            return Regex.Replace(line, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1");
        }
    }
}
