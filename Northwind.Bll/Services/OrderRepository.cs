using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public override Task<IEnumerable<Order>> GetListAsync()
        {
            return Task.FromResult(DbContext.Orders.Include(x => x.OrderDetails).AsEnumerable());
        }

        public override Task<IEnumerable<Order>> GetListForAsync(object? foreignKeys)
        {
            var ids = (Tuple<string?, string?>)(foreignKeys ?? (null as string, null as string));

            var orders = ids switch
            {
                (var x, var y) when !x.IsNullOrEmpty() && y == null => DbContext.Orders.AsNoTracking().Include(x => x.OrderDetails).Where(x => x.CustomerId == ids.Item1),
                (var x, var y) when x.IsNullOrEmpty() && int.TryParse(y, out int employeeId) && employeeId > 0 => DbContext.Orders.AsNoTracking().Include(x => x.OrderDetails).Where(x => x.EmployeeId == int.Parse(ids.Item2 ?? "0")),
                _ => DbContext.Orders.AsNoTracking().Include(x => x.OrderDetails)
            };

            return Task.FromResult(orders.AsEnumerable());
        }

        public override async Task<Order?> DeleteAsync(object? id)
        {
            var order = await DbContext.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.OrderId == (int?)id);

            if (order != null)
            {
                if (!order.OrderDetails.IsNullOrEmpty())
                {
                    DbContext.OrderDetails.RemoveRange(order.OrderDetails);
                }
                
                DbContext.Orders.Remove(order);
                await DbContext.SaveChangesAsync();
            }
            
            return order;
        }

        public override async Task<int> DeleteSeveralAsync(int[] ids)
        {
            int count = 0;

            foreach (var id in ids!)
            {
                await DeleteAsync(id);
                count++;
            }

            return count;
        }
    }
}
