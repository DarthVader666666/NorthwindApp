using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Northwind.Bll.Validation
{
    public sealed class DateValidationAttribute : ValidationAttribute
    {
        public int MinYears { get; set; } = 18;
        public int MaxYears { get; set; } = 60;

        public DateValidationAttribute()
        {
        }

        public DateValidationAttribute(int minYears, int maxYears)
        {
            MinYears = minYears;
            MaxYears = maxYears;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            DateTime minimum = DateTime.UtcNow.AddYears(-MaxYears);
            DateTime maximum = DateTime.UtcNow.AddYears(-MinYears);

            var s = value as string;

            if (s != null && string.IsNullOrEmpty(s))
            {
                return true;
            }

            var min = (IComparable)minimum;
            var max = (IComparable)maximum;

            return min.CompareTo(value) <= 0 && max.CompareTo(value) >= 0;
        }

        public override string FormatErrorMessage(string name) => string.Format(CultureInfo.CurrentCulture, $"Must be between {MinYears} and {MaxYears} years", name);
    }
}
