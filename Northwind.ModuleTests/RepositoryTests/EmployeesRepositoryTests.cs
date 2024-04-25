using Moq;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;
using Xunit;

namespace Northwind.ModuleTests.RepositoryTests
{
    public class EmployeesRepositoryTests
    {
        private readonly Random random = new Random();
        private readonly Mock<IRepository<Employee>> repository = new Mock<IRepository<Employee>>();

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async Task GetAllEmployees_Test(int amount)
        {
            repository.Setup(mock => mock.GetListAsync()).ReturnsAsync(await Task.FromResult(DatabaseSeeder.GenerateEmployees(amount)));

            var employees = await repository.Object.GetListAsync();

            Assert.NotNull(employees);
            Assert.NotEmpty(employees);
            Assert.Equal(amount, employees.Count());
        }

        [Theory]
        [MemberData(nameof(Employees))]
        public async Task GetEmployeeById_Test(Employee? employee)
        {
            employee!.EmployeeId = random.Next(1, 10);
            int? id = employee.EmployeeId;
            repository.Setup(mock => mock.GetAsync(It.Is<int?>(x => x == id))).ReturnsAsync(await Task.FromResult<Employee?>(employee));

            var actual = await repository.Object.GetAsync(id);

            Assert.NotNull(actual);
            Assert.Equal(employee.EmployeeId, actual.EmployeeId);
        }

        [Theory]
        [MemberData(nameof(Employees))]
        public async Task CreateEmployee_Test(Employee employee)
        {
            var id = random.Next(1, 10);
            employee.EmployeeId = id;
            repository.Setup(mock => mock.CreateAsync(It.Is<Employee>(x => x == employee))).ReturnsAsync(await Task.FromResult(employee));

            var actual = await repository.Object.CreateAsync(employee);

            Assert.NotNull(actual);
            Assert.Equal(id, actual.EmployeeId);
            Assert.Equal(employee.FirstName, actual.FirstName);
            Assert.Equal(employee.LastName, actual.LastName);
            Assert.Equal(employee.BirthDate, actual.BirthDate);
            Assert.Equal(employee.HireDate, actual.HireDate);
        }

        [Theory]
        [MemberData(nameof(Employees))]
        public async Task DeleteEmployee_Test(Employee employee)
        {
            employee.EmployeeId = random.Next(1, 10);
            var id = employee.EmployeeId;
            repository.Setup(mock => mock.DeleteAsync(It.Is<int>(x => x == id))).ReturnsAsync(await Task.FromResult(employee));

            var actual = await repository.Object.DeleteAsync(id);

            Assert.NotNull(actual);
            Assert.Equal(employee.EmployeeId, actual.EmployeeId);
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
