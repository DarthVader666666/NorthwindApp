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
        private const int amountOfCategories = 15;
        private static readonly string[] jobTitles =
        {
            "Principal Integration Producer",
            "Senior Implementation Representative",
            "Global Impact Synergist",
            "Senior Usability Associate",
            "Customer Ideation Developer",
            "Forward Optimization Officer",
            "Dynamic Directives Associate",
            "Future Infrastructure Representative",
            "Corporate Marketing Engineer",
            "Central Markets Producer"
        };

        public static async Task SeedDatabase<TDbContext>(this TDbContext dbContext) where TDbContext : NorthwindDbContext
        {
            if (dbContext == null)
            {
                return;
            }

            try
            {
                await dbContext.Employees.AddRangeAsync(GenerateEmployees());
                await dbContext.Categories.AddRangeAsync(GenerateCategories());
                await dbContext.SaveChangesAsync();

                foreach (var item in dbContext.Employees)
                {
                    var ids = dbContext.Employees.Select(e => e.EmployeeId);
                    int? id = null;

                    while (id == null || id == item.EmployeeId)
                    {
                        id = new Random().Next(ids.Min(), ids.Max());
                    }

                    item.ReportsTo = id;
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<Employee> GenerateEmployees(int count = amountOfEmployees)
        {
            return new Faker<Employee>()
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Title, f => jobTitles[random.Next(0, jobTitles.Length)])
                .RuleFor(e => e.HireDate, f => f.Date.Between(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow))
                .RuleFor(e => e.BirthDate, f => f.Person.DateOfBirth)
                .RuleFor(e => e.Photo, f => DownloadPicture(f.Person.Avatar))
                .Generate(count);
        }

        public static List<Category> GenerateCategories(int count = amountOfCategories)
        {
            return new Faker<Category>()
                .RuleFor(c => c.CategoryName, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.Description, f => f.Commerce.ProductDescription())
                .RuleFor(c => c.Picture, f => DownloadPicture($"wwwroot/pics/categories/{random.Next(1, 6)}.png", ImageHeaders.Category))
                .Generate(amountOfCategories);
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
