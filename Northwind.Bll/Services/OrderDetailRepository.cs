using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Northwind.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Northwind.Bll.Services
{
    public class OrderDetailRepository : RepositoryBase<OrderDetail, NorthwindDbContext>
    {
        public OrderDetailRepository(NorthwindDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<OrderDetail?> GetAsync(object id)
        {
            var ids = ((int orderId, int productId))id;

            return await DbContext.OrderDetails.Include(x => x.Product).FirstOrDefaultAsync(x => x.OrderId == ids.orderId && x.ProductId == ids.productId);
        }

        public override Task<IEnumerable<OrderDetail?>> GetListForAsync(int fkId)
        {
            var orderDetails = DbContext.OrderDetails.Include(x => x.Product).Include(x => x.Order).Where(x => x.OrderId == fkId).AsEnumerable();

            return Task.Run(() => 
            {
                if (orderDetails.IsNullOrEmpty())
                { 
                    return Enumerable.Empty<OrderDetail?>();
                }

                return orderDetails;
            });
        }
    }
}
