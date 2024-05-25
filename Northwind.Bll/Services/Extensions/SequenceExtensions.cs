using Microsoft.IdentityModel.Tokens;

namespace Northwind.Bll.Services.Extensions
{
    public static class SequenceExtensions
    {
        public static Dictionary<string, int>? GetColumnWidths<T>(this IEnumerable<T> sequence) where T : class
        {
            var item = sequence?.FirstOrDefault(x => x != null);
            var propertyNames = item?.GetType()?.GetProperties().Select(x => x.Name).Where(x => !x.Contains("Id"));
            var dictionary = new Dictionary<string, int>();

            if (sequence.IsNullOrEmpty() || propertyNames.IsNullOrEmpty())
            {
                return dictionary;
            }

            foreach (var name in propertyNames!)
            {
                dictionary[name] = sequence!.AsParallel().Where(x => x != null).Max(x => x?.GetType()?.GetProperty(name)?.GetValue(x)?.ToString()?.Length ?? 0);
            }

            var width = dictionary.Sum(x => x.Value);

            foreach (var name in propertyNames)
            {
                dictionary[name] = (int)(dictionary[name] / (float)width * 100);
            }

            return dictionary;
        }
    }
}
