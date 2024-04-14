using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;
using Xunit;

namespace Northwind.ModuleTests.RepositoryTests
{
    public class EmployeesRepositoryTests : IClassFixture<DbContextFactory>
    {
        private readonly DbContextFactory _contextFactory;

        public EmployeesRepositoryTests(DbContextFactory dbContextFactory)
        {
            _contextFactory = dbContextFactory;
        }

        [Fact]
        public void GetAllEmployees_Test()
        {
            var repository = GetEmployeeRepository(true);

            var employees = repository.GetList();

            Assert.NotNull(employees);
            Assert.NotEmpty(employees);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetEmployeeByIds_Test(int? id)
        {
            var repository = GetEmployeeRepository(true);

            var employee = repository.Get(id);

            Assert.NotNull(employee);
        }

        [Theory]
        [MemberData(nameof(Employees))]
        public async Task CreateEmployee_Test(Employee employee)
        {
            var repository = GetEmployeeRepository(false);

            var actual = await repository.Create(employee);

            Assert.NotNull(actual);
            Assert.Equal(employee.FirstName, actual.FirstName);
            Assert.Equal(employee.LastName, actual.LastName);
            Assert.Equal(employee.BirthDate, actual.BirthDate);
            Assert.Equal(employee.HireDate, actual.HireDate);
        }

        [Fact]
        public async Task DeleteEmployee_Test()
        {
            var repository = GetEmployeeRepository(true);

            var employees = repository.GetList();

            foreach (var employee in employees)
            { 
                var actual = await repository.Delete(employee!.EmployeeId);

                Assert.NotNull(actual);
                Assert.Equal(employee.EmployeeId, actual.EmployeeId);
                Assert.Null(await repository.Get(employee.EmployeeId));
            }            
        }

        private IRepository<Employee> GetEmployeeRepository(bool seed = false)
        {
            var context = _contextFactory.GetInMemoryDbContext();

            if (seed)
            { 
                context.SeedDatabase().Wait();
            }

            return new EmployeeRepository(context);
        }

        public static IEnumerable<object[]> Employees
        {
            get
            {
                var employees = new List<object[]>();

                foreach (var item in DatabaseSeeder.GenerateEmployees())
                {
                    employees.Add([item]);
                }

                return employees;
            }
        }
    }
}
