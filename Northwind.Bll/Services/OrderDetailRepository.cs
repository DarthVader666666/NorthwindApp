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
