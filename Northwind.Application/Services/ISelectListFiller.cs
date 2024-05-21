using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Bll.Services;

namespace Northwind.Application.Services
{
    public interface ISelectListFiller
    {
        void FillSelectLists<TModel>(TModel? model, int? employeeId = null, int? shipperId = null, string? customerId = null, int? categoryId = null, 
            int? supplierId = null, string? userId = null) where TModel : class;

        public SelectList? GetCategoryIdSelectList(int? categoryId, bool all = false);

        public SelectList? GetSupplierIdSelectList(int? supplierId, bool all = false);

        public SelectList? GetSelectList<TKey>(IDictionary<TKey, string> dictionary, object? id, bool all = false);
    }
}