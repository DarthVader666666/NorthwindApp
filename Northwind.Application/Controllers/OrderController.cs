﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Northwind.Application.Models.Order;
using Northwind.Application.Models.Product;
using Northwind.Application.Models;
using Northwind.Bll.Services;
using System.Drawing.Printing;
using Microsoft.IdentityModel.Tokens;

namespace Northwind.Application.Controllers
{
    //[Authorize(Roles ="admin, customer")]
    public class OrderController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Shipper> _shipperRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private const int pageSize = 6; 

        public OrderController(IRepository<Order> orderRepository, IRepository<Customer> customerRepository, IRepository<Shipper> shipperRepository,
            IRepository<Employee> employeeRepository, IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _shipperRepository = shipperRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index(string fkId = "", int page = 1)
        {
            var allOrders = await _orderRepository.GetListForAsync(fkId);
            var orders = allOrders.Skip((page - 1) * pageSize).Take(pageSize);
            var orderDataModels = _mapper.Map<IEnumerable<OrderIndexDataModel>>(orders);

            var pageModel = new PageViewModel(allOrders.Count(), page, pageSize, fkId);
            var orderIndexModel = new OrderIndexModel(orderDataModels, pageModel);

            if (!fkId.IsNullOrEmpty())
            { 
                ViewBag.PreviousPage = Url.ActionLink("Details", "Customer", new { id = fkId });
            }

            ViewBag.Id = fkId;
            var customer = await _customerRepository.GetAsync(fkId);
            ViewBag.CompanyName = customer != null ? customer.CompanyName : "";

            return View(orderIndexModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.GetAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDetailsModel = _mapper.Map<OrderDetailsModel>(order);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Order", new { fkId = order.CustomerId });

            return View(orderDetailsModel);
        }

        public IActionResult Create(string fkId)
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Order", new { fkId = fkId });

            var orderCreateModel = new OrderCreateModel();
            orderCreateModel.EmployeeIdList = GetEmployeeIdSelectList();
            orderCreateModel.CustomerIdList = GetCustomerIdSelectList(fkId);
            orderCreateModel.ShipperIdList = GetShipperIdSelectList();

            return View(orderCreateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateModel orderCreateModel)
        {
            if (ModelState.IsValid)
            {
                var order = _mapper.Map<Order>(orderCreateModel);
                await _orderRepository.CreateAsync(order);

                return RedirectToAction(nameof(Index), new { fkId = order.CustomerId });
            }

            orderCreateModel.EmployeeIdList = GetEmployeeIdSelectList(orderCreateModel.EmployeeId);
            orderCreateModel.CustomerIdList = GetCustomerIdSelectList(orderCreateModel.CustomerId);
            orderCreateModel.ShipperIdList = GetShipperIdSelectList(orderCreateModel.ShipperId);

            return View(orderCreateModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderEditModel = _mapper.Map<OrderEditModel>(await _orderRepository.GetAsync(id));

            if (orderEditModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            orderEditModel.EmployeeIdList = GetEmployeeIdSelectList(orderEditModel.EmployeeId);
            orderEditModel.CustomerIdList = GetCustomerIdSelectList(orderEditModel.CustomerId);
            orderEditModel.ShipperIdList = GetShipperIdSelectList(orderEditModel.ShipperId);

            return View(orderEditModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderEditModel orderEditModel)
        {
            if (id != orderEditModel.OrderId)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order = _mapper.Map<Order>(orderEditModel);
                    await _orderRepository.UpdateAsync(order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OrderExists(orderEditModel.OrderId))
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

            orderEditModel.EmployeeIdList = GetEmployeeIdSelectList(orderEditModel.EmployeeId);
            orderEditModel.CustomerIdList = GetCustomerIdSelectList(orderEditModel.CustomerId);
            orderEditModel.ShipperIdList = GetShipperIdSelectList(orderEditModel.ShipperId);

            return View(orderEditModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var orders = await _orderRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Order");

            return View(_mapper.Map<IEnumerable<OrderIndexDataModel>>(orders));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _orderRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> OrderExists(int id)
        {
            return (await _orderRepository.GetAsync(id)) != null;
        }

        private SelectList GetEmployeeIdSelectList(int? selectedEmployeeId = null)
        {
            var employees = _employeeRepository.GetListAsync().Result;
            var dictionary = employees.ToDictionary(e => e.EmployeeId, e => e.FirstName + " " + e.LastName);
            dictionary.Add(0, "");

            var selectList = new SelectList(dictionary, "Key", "Value", dictionary);

            SelectListItem selectedItem = null;

            if (selectedEmployeeId != null)
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == selectedEmployeeId.ToString());
            }
            else
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == 0.ToString());
            }

            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }

            return selectList;
        }

        private SelectList GetCustomerIdSelectList(string? selectedCustomerId = null)
        {
            var customer = _customerRepository.GetListAsync().Result;
            var dictionary = customer.ToDictionary(c => c.CustomerId, c => c.CompanyName);
            dictionary.Add("", "");

            var selectList = new SelectList(dictionary, "Key", "Value", dictionary);

            SelectListItem selectedItem = null;

            if (selectedCustomerId != null)
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == selectedCustomerId.ToString());
            }
            else
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == "".ToString());
            }

            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }

            return selectList;
        }

        private SelectList GetShipperIdSelectList(int? selectedShipperId = 0)
        {
            var shippers = _shipperRepository.GetListAsync().Result;
            var dictionary = shippers.ToDictionary(s => s.ShipperId, s => s.CompanyName);
            dictionary.Add(0, "");

            var selectList = new SelectList(dictionary, "Key", "Value", dictionary);

            SelectListItem selectedItem = null;

            if (selectedShipperId != null)
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == selectedShipperId.ToString());
            }
            else
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == 0.ToString());
            }

            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }

            return selectList;
        }
    }
}