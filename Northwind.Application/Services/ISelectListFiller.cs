using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Northwind.Bll.Services;

namespace Northwind.Application.Services
{
    public interface ISelectListFiller
    {
        void FillSelectLists<TModel>(TModel? model, int? sellerId = null, int? shipperId = null, string? customerId = null, int? categoryId = null, 
            int? supplierId = null, string? userId = null) where TModel : class;

        public SelectList? GetCategoryIdSelectList(int? categoryId, bool all = false);
        public SelectList? GetSupplierIdSelectList(int? supplierId, bool all = false);
        public SelectList? GetSelectList<TKey>(IDictionary<TKey, string> dictionary, object? id, bool all = false);
        public SelectList? GetSellerIdSelectList(int? sellerId = null, string? userId = null, bool all = false);
        public SelectList? GetCustomerIdSelectList(string? customerId = null, string? userId = null, bool all = false);
    }
}