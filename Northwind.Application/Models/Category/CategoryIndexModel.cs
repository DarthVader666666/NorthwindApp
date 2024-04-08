namespace Northwind.Application.Models.Category
{
    public class CategoryIndexModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public byte[]? Picture { get; set; }

        public bool Delete { get; set; }
    }
}
