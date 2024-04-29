using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Northwind.Bll.Validation
{
    public class IdExistsValidationAttribute: ValidationAttribute
    {
        private readonly IRepository<Customer> _customerRepository;

        public IdExistsValidationAttribute(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            var s = value as string;

            if (s == null || string.IsNullOrEmpty(s))
            {
                return false;
            }

            return _customerRepository.GetAsync(s.ToUpper()).Result != null;
        }

        public override string FormatErrorMessage(string name) => string.Format(CultureInfo.CurrentCulture, $"Current CustomerId already exists", name);
    }
}
