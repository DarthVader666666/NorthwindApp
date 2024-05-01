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

            return await DbContext.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.OrderId == (int)id);
        }

        public override Task<IEnumerable<Order?>> GetListForAsync(string fkId)
        {
            if (fkId == null)
            {
                return Task.FromResult<IEnumerable<Order?>>(null);
            }

            return Task.Run(() => DbContext.Orders.Where(x => fkId == "" || x.CustomerId == fkId).AsEnumerable<Order?>());
        }
    }
}
