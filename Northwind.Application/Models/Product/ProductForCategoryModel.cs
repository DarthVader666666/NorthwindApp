﻿using System.Collections;

namespace Northwind.Application.Models.Product
{
    public class ProductForCategoryModel : IEnumerable<ProductIndexModel>
    {
        public IEnumerable<ProductIndexModel>? Products { get; set; }
        public string? CategoryName { get; set; }
        public int CategoryId { get; set; }

        public IEnumerator<ProductIndexModel> GetEnumerator()
        {
            foreach (var item in Products!)
            {
                yield return item; 
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}