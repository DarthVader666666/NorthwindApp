namespace Northwind.Application.Models.Category
{
    public class CategoryEditModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public byte[]? Picture { get; set; }

        public IFormFile? FormFile { get; set; }
    }
}
