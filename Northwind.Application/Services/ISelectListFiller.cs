namespace Northwind.Application.Services
{
    public interface ISelectListFiller
    {
        void FillSelectLists<TModel>(TModel? model, int? employeeId = null, int? shipperId = null, string? customerId = null,
            int? categoryId = null, int? supplierId = null) where TModel : class;
    }
}