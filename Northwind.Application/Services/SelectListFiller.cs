using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Models.Order;
using Northwind.Application.Models.Product;
using Northwind.Application.Models.Roles;
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
        private readonly UserManager<NorthwindUser> _userManager;

        public SelectListFiller(IRepository<Employee> employeeRepository, IRepository<Customer> customerRepository, IRepository<Shipper> shipperRepository,
            IRepository<Supplier> supplierRepository, IRepository<Category> categoryRepository, UserManager<NorthwindUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _customerRepository = customerRepository;
            _shipperRepository = shipperRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public void FillSelectLists<TModel>(TModel? model, int? employeeId = null, int? shipperId = null, string? customerId = null, int? categoryId = null, 
            int? supplierId = null, string? userId = null) where TModel : class
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

                orderIndexModel!.CustomerList = GetCustomerIdSelectList(customerId: customerId, userId: null, true);
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

                productIndexModel!.SupplierList = GetSupplierIdSelectList(supplierId, true);
                productIndexModel!.CategoryList = GetCategoryIdSelectList(categoryId, true);
            }

            if (model is RoleChangeModel)
            {
                var roleChangeModel = model as RoleChangeModel;

                roleChangeModel!.CustomerList = GetCustomerIdSelectList(userId: userId);
                roleChangeModel!.EmployeeList = GetEmployeeIdSelectList(userId: userId);
            }
        }

        private SelectList GetEmployeeIdSelectList(int? employeeId = null, string? userId = null, bool all = false)
        {
            if (!userId.IsNullOrEmpty())
            {
                var user = _userManager.FindByIdAsync(userId!).Result;
                employeeId = user?.EmployeeId;
            }

            var employees = _employeeRepository.GetListAsync();
            Task.WaitAny(employees);

            var dictionary = employees.Result.ToDictionary(e => e?.EmployeeId ?? 0, e => $"{e.FirstName} {e.LastName} - {e.Title}");

            return GetSelectList(dictionary, employeeId, all);
        }

        private SelectList GetCustomerIdSelectList(string? customerId = null, string? userId = null, bool all = false)
        {
            if (!userId.IsNullOrEmpty())
            {
                var user = _userManager.FindByIdAsync(userId!).Result;
                customerId = user?.CustomerId;
            }

            var customers = _customerRepository.GetListAsync();
            Task.WaitAny(customers);

            var dictionary = customers.Result.ToDictionary(c => c?.CustomerId ?? "", c => c?.CompanyName ?? "");

            return GetSelectList(dictionary, customerId, all);
        }

        private SelectList GetShipperIdSelectList(int? shipperId)
        {
            var shippers = _shipperRepository.GetListAsync().Result;
            var dictionary = shippers.ToDictionary(s => s?.ShipperId ?? 0, s => s?.CompanyName ?? "");

            return GetSelectList(dictionary, shipperId);
        }

        public SelectList? GetCategoryIdSelectList(int? categoryId, bool all = false)
        {
            var categories = _categoryRepository.GetListAsync().Result;
            var dictionary = categories.ToDictionary(c => c?.CategoryId ?? 0, c => c?.CategoryName ?? "");

            return GetSelectList(dictionary, categoryId, all: all);
        }

        public SelectList? GetSupplierIdSelectList(int? supplierId, bool all = false)
        {
            var suppliers = _supplierRepository.GetListAsync().Result;
            var dictionary = suppliers.ToDictionary(c => c?.SupplierId ?? 0, c => c?.CompanyName ?? "");

            return GetSelectList(dictionary, supplierId, all);
        }

        public SelectList? GetSelectList<TKey>(IDictionary<TKey, string> dictionary, object? id, bool all = false)
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
