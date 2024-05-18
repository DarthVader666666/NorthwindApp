using Northwind.Application.Enums;
using Northwind.Application.Models.Customer;
using Northwind.Application.Models.Order;
using Northwind.Application.Models.OrderDetail;
using Northwind.Application.Models.Product;

namespace Northwind.Application.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SortSequence<T>(this IEnumerable<T> sequence, SortBy? orderBy, bool desc)
        {
            if (sequence is IEnumerable<OrderIndexDataModel> orderSequence)
            {
                return (IEnumerable<T>)(orderBy switch
                {
                    SortBy.OrderDate => desc ? orderSequence.OrderByDescending(x => x.OrderDate) : orderSequence.OrderBy(x => x.OrderDate),
                    SortBy.OrderStatus => desc ? orderSequence.OrderByDescending(x => x.OrderStatus) : orderSequence.OrderBy(x => x.OrderStatus),
                    SortBy.TotalCost => desc ? orderSequence.OrderByDescending(x => x.TotalCost) : orderSequence.OrderBy(x => x.TotalCost),
                    _ => orderSequence,
                });
            }

            if (sequence is IEnumerable<ProductIndexDataModel> productSequence)
            {
                return (IEnumerable<T>)(orderBy switch
                {
                    SortBy.ProductName => desc ? productSequence.OrderByDescending(x => x.ProductName) : productSequence.OrderBy(x => x.ProductName),
                    SortBy.UnitsInStock => desc ? productSequence.OrderByDescending(x => x.UnitsInStock) : productSequence.OrderBy(x => x.UnitsInStock),
                    SortBy.UnitPrice => desc ? productSequence.OrderByDescending(x => x.UnitPrice) : productSequence.OrderBy(x => x.UnitPrice),
                    SortBy.QuantityPerUnit => desc ? productSequence.OrderByDescending(x => x.QuantityPerUnit) : productSequence.OrderBy(x => x.QuantityPerUnit),
                    _ => productSequence,
                });
            }

            if (sequence is IEnumerable<CustomerIndexDataModel> customerSequence)
            {
                return (IEnumerable<T>)(orderBy switch
                {
                    SortBy.CompanyName => desc ? customerSequence.OrderByDescending(x => x.CompanyName) : customerSequence.OrderBy(x => x.CompanyName),
                    SortBy.ContactName => desc ? customerSequence.OrderByDescending(x => x.ContactName) : customerSequence.OrderBy(x => x.ContactName),
                    SortBy.Country => desc ? customerSequence.OrderByDescending(x => x.Country) : customerSequence.OrderBy(x => x.Country),
                    _ => customerSequence,
                });
            }

            if (sequence is IEnumerable<OrderDetailIndexDataModel> orderDetailSequence)
            {
                return (IEnumerable<T>)(orderBy switch
                {
                    SortBy.TotalPrice => desc ? orderDetailSequence.OrderByDescending(x => x.TotalPrice) : orderDetailSequence.OrderBy(x => x.TotalPrice),
                    SortBy.Quantity => desc ? orderDetailSequence.OrderByDescending(x => x.Quantity) : orderDetailSequence.OrderBy(x => x.Quantity),
                    SortBy.Discount => desc ? orderDetailSequence.OrderByDescending(x => x.Discount) : orderDetailSequence.OrderBy(x => x.Discount),
                    SortBy.ProductName => desc ? orderDetailSequence.OrderByDescending(x => x.ProductName) : orderDetailSequence.OrderBy(x => x.ProductName),
                    _ => orderDetailSequence,
                });
            }

            return sequence;
        }
    }
}
