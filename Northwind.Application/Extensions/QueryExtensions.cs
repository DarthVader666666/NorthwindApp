using Northwind.Application.Enums;
using Northwind.Application.Models.Order;

namespace Northwind.Application.Extensions
{
    public static class QueryExtensions
    {
        public static IEnumerable<T> OrderSequence<T>(this IEnumerable<T> sequence, SortBy? orderBy, bool desc)
        {
            if (sequence is IEnumerable<OrderIndexDataModel> orderSequence)
            {
                return (IEnumerable<T>)(orderBy switch
                {
                    SortBy.OrderDate => desc ? orderSequence.OrderByDescending(x => x.OrderDate) : orderSequence.OrderBy(x => x.OrderDate),
                    SortBy.OrderStatus => desc ? orderSequence.OrderByDescending(x => x.OrderStatus) : orderSequence.OrderBy(x => x.OrderStatus),
                    SortBy.TotalCost => desc ? orderSequence.OrderByDescending(x => x.TotalCost) : orderSequence.OrderBy(x => x.OrderDate),
                    _ => orderSequence,
                });
            }

            return sequence;
        }
    }
}
