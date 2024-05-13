using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class OrderRepository : RepositoryBase<Order, NorthwindDbContext>
    {
        public OrderRepository(NorthwindDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Order?> GetAsync(object? id)
        {
            if (id == null)
            {
                return null;
            }

            return await DbContext.Orders.AsNoTracking().Include(x => x.OrderDetails).Include(x => x.Customer).FirstOrDefaultAsync(x => x.OrderId == (int)id);
        }

        // Gets list of orders for particular customer
        // fkId - CustomerId
        public override Task<IEnumerable<Order?>> GetListForAsync(string? customerId)
        {
            return Task.Run(() => DbContext.Orders.Include(x => x.OrderDetails).Where(x => customerId == null || x.CustomerId == customerId).AsEnumerable<Order?>());
        }

        public override async Task<int> DeleteSeveralAsync(int[] ids)
        {
            int count = 0;

            foreach (var id in ids!)
            {
                var order = await DbContext.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.OrderId == id);

                if (order != null)
                {
                    DbContext.OrderDetails.RemoveRange(order.OrderDetails);
                    await DbContext.SaveChangesAsync();

                    await DeleteAsync(id);
                    count++;
                }
            }

            return count;
        }
    }
}
