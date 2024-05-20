using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Northwind.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Northwind.Bll.Services
{
    public class OrderDetailRepository : RepositoryBase<OrderDetail, NorthwindDbContext>
    {
        private readonly IRepository<Customer> _customerRepository;

        public OrderDetailRepository(IRepository<Customer> customerRepository, NorthwindDbContext dbContext) : base(dbContext)
        {
            _customerRepository = customerRepository;
        }

        public override async Task<OrderDetail?> GetAsync(object? id)
        {
            if (id == null)
            {
                return null;
            }

            var ids = ((int orderId, int productId))id;

            return await DbContext.OrderDetails.AsNoTracking().Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.OrderId == ids.orderId && x.ProductId == ids.productId);
        }

        public override async Task<IEnumerable<OrderDetail?>> GetListForAsync(object? primaryKeys)
        {
            var ids = (Tuple<int?, int?>)(primaryKeys ?? (0, 0));

            var orderDetails = ids switch
            {
                (> 0, null) => DbContext.OrderDetails.Include(x => x.Product).Include(x => x.Order).Where(x => x.OrderId == ids.Item1),
                (null, > 0) => DbContext.OrderDetails.Include(x => x.Product).Include(x => x.Order).Where(x => x.ProductId == ids.Item2),
                _ => null
            };

            if (!orderDetails.IsNullOrEmpty())
            {
                foreach (var item in orderDetails.Select(x => x!.Order).ToList())
                {
                    if (item != null)
                    { 
                        item.Customer = await _customerRepository.GetAsync(item.CustomerId);
                    }
                }
            }

            return orderDetails?.AsEnumerable() ?? Enumerable.Empty<OrderDetail?>();
        }

        public override async Task<int> DeleteSeveralAsync(string[] ids)
        {
            var tuples = ConvertOrderDetailIds(ids);

            foreach (var id in tuples!)
            {
                var item = await DbContext.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == id.orderId && x.ProductId == id.productId);

                if (item != null)
                {
                    DbContext.Remove(item);
                }
            }

            return await DbContext.SaveChangesAsync();
        }

        public override Task<IEnumerable<OrderDetail?>> GetRangeAsync(string[] ids)
        {
            var tuples = ConvertOrderDetailIds(ids).ToArray();

            return Task.Run(() =>
            {
                return GetEntities();

                IEnumerable<OrderDetail?> GetEntities()
                {
                    foreach (var id in tuples)
                    {
                        yield return GetAsync(id).Result;
                    }
                }
            });
        }

        private IEnumerable<(int orderId, int productId)> ConvertOrderDetailIds(string[] ids)
        {
            foreach (var id in ids)
            {
                var couple = id.Split(' ');
                yield return (int.Parse(couple[0]), int.Parse(couple[1]));
            }
        }
    }
}
