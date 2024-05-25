using Northwind.Application.Constants;
using System.Reflection;

namespace Northwind.Application.Services
{
    public static class ViewAdditions
    {
        public static IEnumerable<object?> GetPropertyValues(object? item, IEnumerable<string>? propertyNames = null)
        {
            if (item == null)
            {
                yield return null;
            }

            if (propertyNames == null)
            {
                PropertyInfo[] properties = item?.GetType()?.GetProperties() ?? [];
                propertyNames = properties.Select(x => x.Name).Where(x => !x.Contains("Id"));
            }            

            foreach (var propertyName in propertyNames)
            {
                object? value = null;

                try
                {
                    value = item?.GetType().GetProperty(propertyName)?.GetValue(item);
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                yield return value;
            }
        }

        public static string GetOrderStatusStyle(object? value)
        {
            var color = value switch
            {
                SessionValues.Confirmed => "black",
                SessionValues.InProgress => "goldenrod",
                SessionValues.Completed => "forestgreen",
                SessionValues.NotConfirmed => "gray",
                _ => ""
            };

            return color.Length > 0 ? $"color:{color};font-weight:bold;" : "";
        }
    }
}
