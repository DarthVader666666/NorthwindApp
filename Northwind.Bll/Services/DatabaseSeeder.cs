using Bogus;
using Northwind.Bll.Enums;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public static class DatabaseSeeder
    {
        private static readonly Random random = new Random();
        private const int amountOfEmployees = 5;
        private const int amountOfProducts = 5;
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
                await GenerateEmployees(dbContext);
                await GenerateCategories(dbContext);
                await GenerateProducts(dbContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task<List<Employee>> GenerateEmployees(NorthwindDbContext? dbContext = null, int count = amountOfEmployees)
        {
            var employeeId = 1;

            var employees = new Faker<Employee>()
                .RuleFor(e => e.EmployeeId, f => employeeId++)
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Title, f => jobTitles[random.Next(0, jobTitles.Length)])
                .RuleFor(e => e.HireDate, f => f.Date.Between(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow))
                .RuleFor(e => e.BirthDate, f => f.Person.DateOfBirth)
                .RuleFor(e => e.Photo, f => DownloadPicture(f.Person.Avatar))
                .Generate(count);

            foreach (var item in employees)
            {
                var ids = employees.Select(e => e.EmployeeId).ToList();
                int? id = null;

                if (ids.Count > 1)
                {
                    do
                    {
                        id = new Random().Next(ids.Min(), ids.Max());
                    }
                    while (id == null || id == item.EmployeeId);
                }
                
                item.ReportsTo = id;
            }

            if (dbContext != null)
            {
                dbContext.Employees.AddRange(employees);
                await dbContext.SaveChangesAsync();
            }

            return employees;
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
                    Picture = DownloadPicture($"wwwroot/pics/categories/{name}.png", ImageHeaders.Category),
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
