using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;
using Xunit;

namespace Northwind.Tests.ModuleTests
{
    public class CategoryRepositoryTests : IClassFixture<DbContextFactory>
    {
        private readonly DbContextFactory _contextFactory;

        public CategoryRepositoryTests(DbContextFactory dbContextFactory)
        {
            _contextFactory = dbContextFactory;
        }

        [Fact]
        public void GetAllCategories_Test()
        {
            var repository = GetCategoryRepository(true);

            var categories = repository.GetList();

            Assert.NotNull(categories);
            Assert.NotEmpty(categories);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetCategoryByIds_Test(int? id)
        {
            var repository = GetCategoryRepository(true);

            var employee = repository.Get(id);

            Assert.NotNull(employee);
        }

        [Theory]
        [MemberData(nameof(Categories))]
        public async Task CreateCategory_Test(Category category)
        {
            var repository = GetCategoryRepository(false);

            var actual = await repository.Create(category);

            Assert.NotNull(actual);
            Assert.Equal(category.CategoryName, actual.CategoryName);
            Assert.Equal(category.Description, actual.Description);
        }

        [Fact]
        public async Task DeleteCategory_Test()
        {
            var repository = GetCategoryRepository(true);

            var categories = repository.GetList();

            foreach (var category in categories)
            {
                var actual = await repository.Delete(category!.CategoryId);

                Assert.NotNull(actual);
                Assert.Equal(category.CategoryId, actual.CategoryId);
                Assert.Null(await repository.Get(category.CategoryId));
            }
        }

        private IRepository<Category> GetCategoryRepository(bool seed = false)
        {
            var context = _contextFactory.GetInMemoryDbContext();

            if (seed)
            {
                context.SeedDatabase().Wait();
            }

            return new CategoryRepository(context);
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
