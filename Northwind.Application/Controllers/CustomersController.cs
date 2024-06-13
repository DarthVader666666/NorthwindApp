using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Constants;
using Northwind.Application.Enums;
using Northwind.Application.Extensions;
using Northwind.Application.Models.Customer;
using Northwind.Application.Models.PageModels;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services.Extensions;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin},{UserRoles.Employee}")]
    public class CustomersController : Controller
    {
        private static SortBy? Sort;
        private static bool Desc = false;

        private const int pageSize = 7;

        private readonly IRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;

        public CustomersController(IRepository<Customer> customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int page = 1, string? sortBy = null)
        {
            var customers = await _customerRepository.GetListAsync();
            var customerDataModels = _mapper.Map<IEnumerable<CustomerIndexDataModel>>(customers);
            var columnWidths = customerDataModels.GetColumnWidths();

            if (sortBy != null)
            {
                SortBy? sort = Enum.TryParse(sortBy, out SortBy sortByValue) ? sortByValue : null;

                Desc = Sort == sort ? !Desc : Desc;
                Sort = sort;
            }

            if (Sort != null)
            {
                customerDataModels = customerDataModels.SortSequence(Sort, Desc);
            }

            customerDataModels = customerDataModels.Skip((page - 1) * pageSize).Take(pageSize);

            var pageModel = new PageModelBase(customers.Count(), page, pageSize);
            var productIndexModel = new CustomerIndexModel(customerDataModels, pageModel);
            ViewBag.PageStartNumbering = (page - 1) * pageSize + 1;
            ViewBag.ColumnWidths = columnWidths;

            return View(productIndexModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerDetailsModel = _mapper.Map<CustomerDetailsModel>(customer);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Customers");

            return View(customerDetailsModel);
        }

        [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
        public IActionResult Create()
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Customers");

            return View();
        }

        [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateModel customerCreateModel)
        {
            string customerId;

            do
            {
                customerId = GenerateCustomerId();
            }
            while ((await _customerRepository.GetAsync(customerId)) != null);

            customerCreateModel.CustomerId = customerId;

            if (ModelState.IsValid)
            {
                var customer = _mapper.Map<Customer>(customerCreateModel);
                await _customerRepository.CreateAsync(customer);

                return RedirectToAction(nameof(Index));
            }

            return View(customerCreateModel);
        }

        [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerEditModel = _mapper.Map<CustomerEditModel>(await _customerRepository.GetAsync(id));

            if (customerEditModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(customerEditModel);
        }

        [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CustomerEditModel customerEditModel)
        {
            if (id != customerEditModel.CustomerId)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = _mapper.Map<Customer>(customerEditModel);
                    await _customerRepository.UpdateAsync(customer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CustomerExists(customerEditModel.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(customerEditModel);
        }

        [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            var customers = new List<Customer>();

            foreach (var id in ids)
            {
                var customer = await _customerRepository.GetAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                customers.Add(customer);
            }

            ViewBag.PreviousPage = Url.ActionLink("Index", "Customers");

            return View(_mapper.Map<IEnumerable<CustomerDeleteModel>>(customers));
        }

        [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string[] ids)
        {
            await _customerRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index));
        }

        private static string GenerateCustomerId()
        {
            var guid = Guid.NewGuid();
            return new string(guid.ToString().Where(x => char.IsLetter(x)).ToArray()[..5]).ToUpper();
        }

        private async Task<bool> CustomerExists(string id)
        {
            return (await _customerRepository.GetAsync(id)) != null;
        }
    }
}
