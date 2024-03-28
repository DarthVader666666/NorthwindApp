using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Category
{
    public class CategoryCreateModel
    {
        [Required]
        public string CategoryName { get; set; } = null!;
        [Required]
        public string? Description { get; set; }

        public IFormFile? FormFile { get; set; }
    }
}
