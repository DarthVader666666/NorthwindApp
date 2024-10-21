using Moq;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;
using Xunit;

namespace Northwind.ModuleTests.RepositoryTests
{
    public class SellerRepositoryTests
    {
        private readonly Random random = new Random();
        private readonly Mock<IRepository<Seller>> repository = new Mock<IRepository<Seller>>();

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async Task GetAllSellers_Test(int amount)
        {
            repository.Setup(mock => mock.GetListAsync()).ReturnsAsync(await DatabaseSeeder.GenerateSellers(count: amount));

            var sellers = await repository.Object.GetListAsync();

            Assert.NotNull(sellers);
            Assert.NotEmpty(sellers);
            Assert.Equal(amount, sellers.Count());
        }

        [Theory]
        [MemberData(nameof(Sellers))]
        public async Task GetSellerById_Test(Seller? seller)
        {
            seller!.SellerId = random.Next(1, 10);
            int? id = seller.SellerId;
            repository.Setup(mock => mock.GetAsync(It.Is<int?>(x => x == id))).ReturnsAsync(await Task.FromResult<Seller?>(seller));

            var actual = await repository.Object.GetAsync(id);

            Assert.NotNull(actual);
            Assert.Equal(seller.SellerId, actual.SellerId);
        }

        [Theory]
        [MemberData(nameof(Sellers))]
        public async Task CreateSeller_Test(Seller seller)
        {
            var id = random.Next(1, 10);
            seller.SellerId = id;
            repository.Setup(mock => mock.CreateAsync(It.Is<Seller>(x => x == seller))).ReturnsAsync(await Task.FromResult(seller));

            var actual = await repository.Object.CreateAsync(seller);

            Assert.NotNull(actual);
            Assert.Equal(id, actual.SellerId);
            Assert.Equal(seller.FirstName, actual.FirstName);
            Assert.Equal(seller.LastName, actual.LastName);
            Assert.Equal(seller.BirthDate, actual.BirthDate);
            Assert.Equal(seller.HireDate, actual.HireDate);
        }

        [Theory]
        [MemberData(nameof(Sellers))]
        public async Task DeleteSeller_Test(Seller seller)
        {
            seller.SellerId = random.Next(1, 10);
            var id = seller.SellerId;
            repository.Setup(mock => mock.DeleteAsync(It.Is<int>(x => x == id))).ReturnsAsync(await Task.FromResult(seller));

            var actual = await repository.Object.DeleteAsync(id);

            Assert.NotNull(actual);
            Assert.Equal(seller.SellerId, actual.SellerId);
        }

        public static IEnumerable<object[]> Sellers
        {
            get
            {
                var sellers = new List<object[]>();

                foreach (var item in DatabaseSeeder.GenerateSellers().Result)
                {
                    sellers.Add([item]);
                }

                return sellers;
            }
        }
    }
}
