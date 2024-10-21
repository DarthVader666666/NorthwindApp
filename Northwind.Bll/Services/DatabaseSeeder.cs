using Bogus;
using Bogus.DataSets;
using Northwind.Bll.Enums;
using Northwind.Data;
using Northwind.Data.Entities;
using System.IO;

namespace Northwind.Bll.Services
{
    public static class DatabaseSeeder
    {
        private static readonly Random random = new Random();
        private const int amountOfSellers = 5;
        private const int amountOfProducts = 5;
        private const int amountOfCustomers = 5;
        private static readonly string[] categoryNames = ["Beverages", "Condiments", "Confections", "Dairy Products", "GrainsCereals"];

        private static readonly string[] jobTitles = 
        [
            "Sales Representative",
            "Vice President, Sales",
            "Sales Manager",
            "Inside Sales Coordinator"
        ];

        public static async Task SeedDatabase<TDbContext>(this TDbContext dbContext) where TDbContext : NorthwindDbContext
        {
            if (dbContext == null)
            {
                return;
            }

            try
            {              
                await GenerateSellers(dbContext);
                await GenerateCategories(dbContext);
                await GenerateProducts(dbContext);
                await GenerateCustomers(dbContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task<List<Seller>> GenerateSellers(NorthwindDbContext? dbContext = null, int count = amountOfSellers)
        {
            var sellerId = 1;

            var sellers = new Faker<Seller>()
                .RuleFor(e => e.SellerId, f => sellerId++)
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Title, f => jobTitles[random.Next(0, jobTitles.Length)])
                .RuleFor(e => e.HireDate, f => f.Date.Between(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow))
                .RuleFor(e => e.BirthDate, f => f.Person.DateOfBirth)
                .RuleFor(e => e.Photo, f => FileDownloader.DownloadImage($"wwwroot/pics/sellers/{sellerId}.png"))
                .Generate(count);

            foreach (var item in sellers)
            {
                var ids = sellers.Select(e => e.SellerId).ToList();
                int? id = null;

                if (ids.Count > 1)
                {
                    do
                    {
                        id = new Random().Next(ids.Min(), ids.Max());
                    }
                    while (id == null || id == item.SellerId);
                }
                
                item.ReportsTo = id;
            }

            if (dbContext != null)
            {
                dbContext.Sellers.AddRange(sellers);
                await dbContext.SaveChangesAsync();
            }

            return sellers;
        }

        public static async Task<List<Category>> GenerateCategories(NorthwindDbContext? dbContext = null)
        {
            var categoryId = 1;
            var categories = new List<Category>();

            foreach(var name in categoryNames)
            {
                categories.Add(new Category() 
                {
                    CategoryId = categoryId++,
                    CategoryName = name,
                    Picture = FileDownloader.DownloadImage($"wwwroot/pics/categories/{name}.png"),
                    Description = new Faker().Commerce.ProductDescription()
                });
            }

            if (dbContext != null)
            {
                dbContext.Categories.AddRange(categories);
                await dbContext.SaveChangesAsync();
            }

            return categories;
        }

        public static async Task GenerateProducts(NorthwindDbContext dbContext, int count = amountOfProducts)
        {
            var categories = dbContext.Categories.ToList();
            var productId = 1;

            foreach (var category in categories)
            {
                var products = new Faker<Product>()
                    .RuleFor(p => p.ProductId, f => productId++)
                    .RuleFor(p => p.CategoryId, f => category.CategoryId)
                    .RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
                    .RuleFor(p => p.UnitsInStock, f => f.Random.Short(0, 256))
                    .RuleFor(p => p.UnitPrice, f => f.Random.Decimal(10m, 99.99m))
                    .RuleFor(p => p.QuantityPerUnit, f => $"{f.Commerce.Price(1m, 5m, 0, string.Empty)} units")
                    .Generate(count);

                dbContext.Products.AddRange(products);
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task<List<Customer>> GenerateCustomers(NorthwindDbContext? dbContext = null, int count = amountOfCustomers)
        {
            var customers = new Faker<Customer>()
                .RuleFor(c => c.CustomerId, f => Guid.NewGuid().ToString())
                .RuleFor(c => c.CompanyName, f => f.Company.CompanyName())
                .RuleFor(c => c.ContactName, f => $"{f.Name.FirstName()} {f.Name.LastName()}")
                .RuleFor(c => c.Address, f => f.Address.SecondaryAddress())
                .RuleFor(c => c.Country, f => f.Address.Country())
                .RuleFor(c => c.City, f => f.Address.City())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .Generate(count);

            if (dbContext != null)
            {
                dbContext.Customers.AddRange(customers);
                await dbContext.SaveChangesAsync();
            }

            return customers;
        }

        // Currently unused
        private static byte[]? DownloadPicture(string path, ImageHeaders? imageHeader = null)
        {
            if (imageHeader == ImageHeaders.Category)
            {
                return FileDownloader.DownloadImage(path);
            }

            byte[]? bytes = null;

            using (var client = new HttpClient())
            {
                using var streamTask = client.GetStreamAsync(path);
                streamTask.Wait();

                var stream = streamTask.Result;

                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return bytes;
        }
    }
}
