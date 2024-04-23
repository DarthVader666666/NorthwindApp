namespace Northwind.Application.Models.Category
{
    public class CategoryDetailsModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string? Description { get; set; }

        public byte[]? Picture { get; set; }

        public ICollection<Northwind.Data.Entities.Product> Products { get; set; }
    }
}
