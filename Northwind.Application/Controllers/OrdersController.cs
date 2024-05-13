﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Application.Models.Order;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Northwind.Application.Models.PageModels;
using Northwind.Application.Services;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles ="admin,customer")]
    public class OrdersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ISelectListFiller _selectListFiller;
        private const int pageSize = 6;

        public OrdersController(IRepository<Order> orderRepository, IRepository<Customer> customerRepository, ISelectListFiller selectListFiller, 
            IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _selectListFiller = selectListFiller;
        }
        
        public async Task<IActionResult> Index(string customerId = "", int page = 1)
        {
            var allOrders = await _orderRepository.GetListForAsync(customerId);
            var orders = allOrders.Skip((page - 1) * pageSize).Take(pageSize);
            var orderDataModels = _mapper.Map<IEnumerable<OrderIndexDataModel>>(orders);

            var pageModel = new OrderPageModel(allOrders.Count(), page, pageSize, customerId);
            var orderIndexModel = new OrderIndexModel(orderDataModels, pageModel);

            if (User.IsInRole("admin"))
            {
                _selectListFiller.FillSelectLists(orderIndexModel, customerId: customerId);
            }

            if (!customerId.IsNullOrEmpty())
            { 
                ViewBag.PreviousPage = Url.ActionLink("Details", "Customers", new { id = customerId });
            }

            ViewBag.Id = customerId;
            var customer = await _customerRepository.GetAsync(customerId);
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

            ViewBag.PreviousPage = Url.ActionLink("Index", "Orders", new { customerId = order.CustomerId });

            return View(orderDetailsModel);
        }

        public async Task<IActionResult> Create(string customerId, int? productId = null)
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Orders", new { customerId = customerId });

            var orderCreateModel = new OrderCreateModel();

            _selectListFiller.FillSelectLists(orderCreateModel, customerId: customerId);

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

                return RedirectToAction(nameof(Index), new { customerId = order.CustomerId });
            }

            _selectListFiller.FillSelectLists(orderCreateModel, orderCreateModel.EmployeeId, orderCreateModel.ShipperId, orderCreateModel.CustomerId);

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

            _selectListFiller.FillSelectLists(orderEditModel, orderEditModel.EmployeeId, orderEditModel.ShipperId, orderEditModel.CustomerId);

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

                return RedirectToAction(nameof(Index), new { customerId = orderEditModel.CustomerId });
            }

            _selectListFiller.FillSelectLists(orderEditModel, orderEditModel.EmployeeId, orderEditModel.ShipperId, orderEditModel.CustomerId);

            return View(orderEditModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var orders = await _orderRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Orders");

            return View(_mapper.Map<IEnumerable<OrderIndexDataModel>>(orders));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            var customerId = (await _orderRepository.GetAsync(ids[0])).CustomerId;

            await _orderRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index), new { fkId = customerId });
        }

        private async Task<bool> OrderExists(int id)
        {
            return (await _orderRepository.GetAsync(id)) != null;
        }
    }
}
