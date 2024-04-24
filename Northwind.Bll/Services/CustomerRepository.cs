using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class CustomerRepository : RepositoryBase<Customer, NorthwindDbContext>
    {
        public CustomerRepository(NorthwindDbContext dbContext) : base(dbContext)
        {
        }
    }
}
