using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models;
using Northwind.Application.Models.Category;
using Northwind.Application.Models.Customer;
using Northwind.Application.Models.Employee;
using Northwind.Application.Models.Product;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;
using System.Drawing.Printing;

namespace Northwind.Application.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;
        private const int pageSize = 7;

        public CustomerController(IRepository<Customer> customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var allCustomers = await _customerRepository.GetListAsync();
            var customers = allCustomers.Skip((page - 1) * pageSize).Take(pageSize);
            var customerDataModels = _mapper.Map<IEnumerable<CustomerIndexDataModel>>(customers);

            var pageModel = new PageViewModel(allCustomers.Count(), page, pageSize);
            var productIndexModel = new CustomerIndexModel(customerDataModels, pageModel);

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

            ViewBag.PreviousPage = Url.ActionLink("Index", "Customer");

            return View(customerDetailsModel);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Create");

            return View();
        }

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
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

            ViewBag.PreviousPage = Url.ActionLink("Index", "Customer");

            return View(_mapper.Map<IEnumerable<CustomerDeleteModel>>(customers));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string?[] ids)
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
