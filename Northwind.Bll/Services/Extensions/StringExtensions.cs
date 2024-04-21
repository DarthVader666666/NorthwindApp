using System.Text.RegularExpressions;

namespace Northwind.Bll.Services.Extensions
{
    public static class StringExtensions
    {
        public static string? SplitForUpperCase(this string? line, object? model = null)
        {
            if (line == null && model == null)
            {
                return null;
            }

            line = (line, model) switch
            {
                var (x, y) when x == null && y != null => y.GetType().Name.ToString(),
                var (x, y) when x == null && y == null => null,
                _ => line
            };

            return Regex.Replace(line, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1").TrimStart();
        }
    }
}
