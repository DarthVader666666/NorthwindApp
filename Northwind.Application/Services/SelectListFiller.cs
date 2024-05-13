using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Application.Models.Order;
using Northwind.Application.Models.Product;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Services
{
    internal class SelectListFiller : ISelectListFiller
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Shipper> _shipperRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Category> _categoryRepository;

        public SelectListFiller(IRepository<Employee> employeeRepository, IRepository<Customer> customerRepository, IRepository<Shipper> shipperRepository,
            IRepository<Supplier> supplierRepository, IRepository<Category> categoryRepository)
        {
            _employeeRepository = employeeRepository;
            _customerRepository = customerRepository;
            _shipperRepository = shipperRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;
        }

        public void FillSelectLists<TModel>(TModel? model, int? employeeId = null, int? shipperId = null, string? customerId = null, 
            int? categoryId = null, int? supplierId = null)
            where TModel : class
        {
            if (model == null)
            {
                return;
            }

            if (model is OrderEditModel)
            {
                var orderEditModel = model as OrderEditModel;

                orderEditModel!.EmployeeIdList = GetEmployeeIdSelectList(employeeId);
                orderEditModel.CustomerIdList = GetCustomerIdSelectList(customerId);
                orderEditModel.ShipperIdList = GetShipperIdSelectList(shipperId);
            }

            if (model is OrderCreateModel)
            {
                var orderCreateModel = model as OrderCreateModel;

                orderCreateModel!.EmployeeIdList = GetEmployeeIdSelectList(employeeId);
                orderCreateModel.CustomerIdList = GetCustomerIdSelectList(customerId);
                orderCreateModel.ShipperIdList = GetShipperIdSelectList(shipperId);
            }

            if (model is OrderIndexModel)
            {
                var orderIndexModel = model as OrderIndexModel;

                orderIndexModel!.CustomerList = GetCustomerIdSelectList(customerId, true);
            }

            if (model is ProductCreateModel)
            {
                var productCreateModel = model as ProductCreateModel;

                productCreateModel!.CategoryIdList = GetCategoryIdSelectList(categoryId);
                productCreateModel.SupplierIdList = GetSupplierIdSelectList(supplierId);
            }

            if (model is ProductEditModel)
            {
                var productEditModel = model as ProductEditModel;

                productEditModel!.CategoryIdList = GetCategoryIdSelectList(categoryId);
                productEditModel.SupplierIdList = GetSupplierIdSelectList(supplierId);
            }

            if (model is ProductIndexModel)
            {
                var productIndexModel = model as ProductIndexModel;

                productIndexModel!.CategoryList = GetCategoryIdSelectList(categoryId, true);
            }
        }

        private SelectList GetEmployeeIdSelectList(int? employeeId = null)
        {
            var employees = _employeeRepository.GetListAsync().Result;
            var dictionary = employees.ToDictionary(e => e.EmployeeId, e => e.FirstName + " " + e.LastName);

            return GetSelectList(dictionary, employeeId);
        }

        private SelectList GetCustomerIdSelectList(string? customerId = null, bool all = false)
        {
            var customer = _customerRepository.GetListAsync().Result;
            var dictionary = customer.ToDictionary(c => c.CustomerId, c => c.CompanyName);

            return GetSelectList(dictionary, customerId, all: all);
        }

        private SelectList GetShipperIdSelectList(int? shipperId = 0)
        {
            var shippers = _shipperRepository.GetListAsync().Result;
            var dictionary = shippers.ToDictionary(s => s.ShipperId, s => s.CompanyName);

            return GetSelectList(dictionary, shipperId);
        }

        private SelectList GetCategoryIdSelectList(int? categoryId = null, bool all = false)
        {
            var categories = _categoryRepository.GetListAsync().Result;
            var dictionary = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);

            return GetSelectList(dictionary, categoryId, all: all);
        }

        private SelectList GetSupplierIdSelectList(int? supplierId = null)
        {
            var suppliers = _supplierRepository.GetListAsync().Result;
            var dictionary = suppliers.ToDictionary(c => c.SupplierId, c => c.CompanyName);

            return GetSelectList(dictionary, supplierId);
        }

        SelectList GetSelectList<TKey>(IDictionary<TKey,string> dictionary, object? id, bool all = false)
        {
            dynamic defaultKeyValue = typeof(TKey) == typeof(int) ? 0 : "";
            dictionary.Add(defaultKeyValue, all ? "All" : "");
            var selectList = new SelectList(dictionary, "Key", "Value", dictionary);
            SelectListItem? selectedItem = null;

            if (id != null)
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == ((TKey)id).ToString());
            }
            else
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == (defaultKeyValue is int ? ((int)defaultKeyValue).ToString() : (string)defaultKeyValue));
            }

            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }

            return selectList;
        }
    }
}
