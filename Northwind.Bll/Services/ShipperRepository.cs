using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class ShipperRepository:RepositoryBase<Shipper, NorthwindDbContext>
    {
        public ShipperRepository(NorthwindDbContext dbContext) : base(dbContext)
        {
        }
    }
}
