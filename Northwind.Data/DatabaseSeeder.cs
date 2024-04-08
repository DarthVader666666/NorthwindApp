using Bogus;
using Northwind.Data.Entities;

namespace Northwind.Data
{
    public static class DatabaseSeeder
    {
        private const int amountOfEmployees = 5;
        private const int amountOfCategories = 5;

        public static async Task SeedDatabase<TDbContext>(this TDbContext dbContext) where TDbContext : NorthwindDbContext
        {
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

        private static List<Employee> GenerateEmployees()
        { 
            return new Faker<Employee>()
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Title, f => f.Hacker.Phrase())
                .RuleFor(e => e.HireDate, f => f.Date.Between(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow))
                .RuleFor(e => e.BirthDate, f => f.Person.DateOfBirth)
                .RuleFor(e => e.Photo, f => DownloadPicture(f.Person.Avatar))
                .Generate(amountOfEmployees);
        }

        private static List<Category> GenerateCategories()
        {
            return new Faker<Category>()
                .RuleFor(c => c.CategoryName, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.Description, f => f.Commerce.ProductDescription())
                .RuleFor(c => c.Picture, f => DownloadPicture(f.Person.Avatar))
                .Generate(amountOfCategories);
        }

        private static byte[]? DownloadPicture(string url)
        {
            byte[]? bytes = null;

            using (var client = new HttpClient())
            {
                using var streamTask = client.GetStreamAsync(url);
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
