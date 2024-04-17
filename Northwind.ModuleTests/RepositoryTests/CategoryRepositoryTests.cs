using Moq;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;
using Xunit;

namespace Northwind.ModuleTests.RepositoryTests
{
    public class CategoryRepositoryTests
    {
        private readonly Random random = new Random();
        private readonly Mock<IRepository<Category>> repository = new Mock<IRepository<Category>>();

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void GetAllCategories_Test(int amount)
        {
            repository.Setup(mock => mock.GetList()).Returns(DatabaseSeeder.GenerateCategories(amount));

            var categories = repository.Object.GetList();

            Assert.NotNull(categories);
            Assert.NotEmpty(categories);
        }

        [Theory]
        [MemberData(nameof(Categories))]
        public async Task GetCategoryByIds_Test(Category category)
        {
            var id = random.Next(1, 10);
            category.CategoryId = id;
            repository.Setup(mock => mock.GetAsync(It.Is<int>(x => x == id))).ReturnsAsync(await Task.FromResult(category));

            var actual = await repository.Object.GetAsync(id);

            Assert.NotNull(actual);
            Assert.Equal(id, actual.CategoryId);
        }

        [Theory]
        [MemberData(nameof(Categories))]
        public async Task CreateCategory_Test(Category category)
        {
            var id = random.Next(1, 10);
            category.CategoryId = id;
            repository.Setup(mock => mock.CreateAsync(It.Is<Category>(x => x == category))).ReturnsAsync(await Task.FromResult(category));

            var actual = await repository.Object.CreateAsync(category);

            Assert.NotNull(actual);
            Assert.Equal(category.CategoryName, actual.CategoryName);
            Assert.Equal(category.Description, actual.Description);
        }

        [Theory]
        [MemberData(nameof(Categories))]
        public async Task DeleteCategory_Test(Category category)
        {
            var id = random.Next(1, 10);
            category.CategoryId = id;
            repository.Setup(mock => mock.DeleteAsync(It.Is<int?>(x => x == id))).ReturnsAsync(await Task.FromResult(category));

            var actual = await repository.Object.DeleteAsync(category!.CategoryId);

            Assert.NotNull(actual);
            Assert.Equal(category.CategoryId, actual.CategoryId);
        }

        public static IEnumerable<object[]> Categories
        {
            get
            {
                var employees = new List<object[]>();

                foreach (var item in DatabaseSeeder.GenerateCategories())
                {
                    employees.Add([item]);
                }

                return employees;
            }
        }
    }
}
