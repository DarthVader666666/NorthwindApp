﻿using Northwind.Bll.Interfaces;
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

        public override async Task<OrderDetail?> GetAsync(object id)
        {
            var ids = ((int orderId, int productId))id;

            return await DbContext.OrderDetails.Include(x => x.Product).FirstOrDefaultAsync(x => x.OrderId == ids.orderId && x.ProductId == ids.productId);
        }

        public override async Task<IEnumerable<OrderDetail?>> GetListForAsync(string? primaryKeys)
        {
            var couple = primaryKeys.IsNullOrEmpty() ? new string[]{ "0", "0" } : primaryKeys!.Split(' ');
            (int orderId, int productId) ids = (int.Parse(couple[0]), int.Parse(couple[1]));

            List<OrderDetail> orderDetails = ids switch
            {
                (> 0, 0) => DbContext.OrderDetails.Include(x => x.Product).Include(x => x.Order).Where(x => x.OrderId == ids.orderId).ToList(),
                (0, > 0) => DbContext.OrderDetails.Include(x => x.Product).Include(x => x.Order).Where(x => x.ProductId == ids.productId).ToList(),
                _ => new List<OrderDetail>()
            };

            if (!orderDetails.IsNullOrEmpty())
            {
                foreach (var item in orderDetails.Select(x => x!.Order))
                {
                    if (item != null)
                    { 
                        item.Customer = await _customerRepository.GetAsync(item.CustomerId);
                    }
                }
            }

            return orderDetails;
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