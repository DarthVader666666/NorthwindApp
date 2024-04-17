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
    }
}
