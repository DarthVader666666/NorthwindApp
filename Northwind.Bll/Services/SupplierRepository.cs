using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class SupplierRepository : RepositoryBase<Supplier, NorthwindDbContext>
    {
        public SupplierRepository(NorthwindDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Supplier?> DeleteAsync(object? id)
        {
            var supplier = await DbContext.Suppliers.Include(x => x.Products).FirstOrDefaultAsync(x => x.SupplierId == (int?)id);

            if (supplier != null)
            {
                if (!supplier.Products.IsNullOrEmpty())
                {
                    DbContext.Products.RemoveRange(supplier.Products);
                }

                DbContext.Suppliers.Remove(supplier);
                await DbContext.SaveChangesAsync();
            }

            return supplier;
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
