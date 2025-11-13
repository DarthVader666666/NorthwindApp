//using Microsoft.Extensions.DependencyInjection;

//using Moq;
//using Northwind.Bll.Interfaces;
//using Northwind.Bll.Services;
//using Northwind.Data;
//using Northwind.Data.Entities;
//using Xunit;

//namespace Northwind.ModuleTests.RepositoryTests
//{
//    public class CategoryRepositoryTests
//    {
//        private readonly Random random = new Random();
//        private readonly Mock<IRepository<Category>> repository = new Mock<IRepository<Category>>();
//        private readonly NorthwindDbContext _context;
//        private static IEnumerable<Category> _categories;

//        public CategoryRepositoryTests(IServiceProvider serviceProvider)
//        {
//            using var scope = serviceProvider.CreateScope();
//            _context = scope.ServiceProvider.GetService<NorthwindDbContext>();
//            DatabaseSeeder.SeedCategories(_context);
//            _categories = _context.Categories;
//        }

//        [Fact]
//        public async Task GetAllCategories_Test()
//        {
//            repository.Setup(mock => mock.GetListAsync()).ReturnsAsync(_context.Categories);

//            var categories = await repository.Object.GetListAsync();

//            Assert.NotNull(categories);
//            Assert.NotEmpty(categories);
//        }

//        [Theory]
//        [MemberData(nameof(Categories))]
//        public async Task GetCategoryByIds_Test(Category category)
//        {
//            var id = random.Next(1, 10);
//            category.CategoryId = id;
//            repository.Setup(mock => mock.GetAsync(It.Is<int>(x => x == id))).ReturnsAsync(await Task.FromResult(category));

//            var actual = await repository.Object.GetAsync(id);

//            Assert.NotNull(actual);
//            Assert.Equal(id, actual.CategoryId);
//        }

//        [Theory]
//        [MemberData(nameof(Categories))]
//        public async Task CreateCategory_Test(Category category)
//        {
//            var id = random.Next(1, 10);
//            category.CategoryId = id;
//            repository.Setup(mock => mock.CreateAsync(It.Is<Category>(x => x == category))).ReturnsAsync(await Task.FromResult(category));

//            var actual = await repository.Object.CreateAsync(category);

//            Assert.NotNull(actual);
//            Assert.Equal(category.CategoryName, actual.CategoryName);
//            Assert.Equal(category.Description, actual.Description);
//        }

//        [Theory]
//        [MemberData(nameof(Categories))]
//        public async Task DeleteCategory_Test(Category category)
//        {
//            var id = random.Next(1, 10);
//            category.CategoryId = id;
//            repository.Setup(mock => mock.DeleteAsync(It.Is<int?>(x => x == id))).ReturnsAsync(await Task.FromResult(category));

//            var actual = await repository.Object.DeleteAsync(category!.CategoryId);

//            Assert.NotNull(actual);
//            Assert.Equal(category.CategoryId, actual.CategoryId);
//        }

//        public static IEnumerable<object[]> Categories
//        {
//            get
//            {
//                var categories = new List<object[]>();

//                foreach (var item in _categories)
//                {
//                    categories.Add([item]);
//                }

//                return categories;
//            }
//        }
//    }
//}
