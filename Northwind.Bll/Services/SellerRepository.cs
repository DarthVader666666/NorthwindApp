using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class SellerRepository : RepositoryBase<Seller, NorthwindDbContext>
    {
        public SellerRepository(NorthwindDbContext context) : base (context)
        {
        }

        public async override Task<Seller?> GetAsync(object id)
        {
            return await DbContext.Sellers.Include(e => e.ReportsToNavigation).FirstOrDefaultAsync(e => e.SellerId == (int?)id);
        }
    }
}
