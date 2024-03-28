﻿using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Category
{
    public class CategoryEditModel
    {
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; } = null!;
        [Required]
        public string? Description { get; set; }

        public byte[]? Picture { get; set; }

        public IFormFile? FormFile { get; set; }
    }
}
