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

        public override Task<IEnumerable<Order?>> GetListForAsync(string fkId)
        {
            return Task.Run(() => DbContext.Orders.Where(x => fkId == "" || x.CustomerId == fkId).AsEnumerable());
        }
    }
}
