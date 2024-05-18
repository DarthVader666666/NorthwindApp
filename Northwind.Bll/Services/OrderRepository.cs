﻿using Microsoft.EntityFrameworkCore;
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

        public override Task<IEnumerable<Order?>> GetListForAsync(object? customerId)
        {
            var query = DbContext.Orders.AsNoTracking().Include(x => x.OrderDetails).Where<Order?>(x => customerId == null || x.CustomerId == customerId as string);
            return Task.Run(() => query.AsEnumerable());
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
