using AutoMapper;
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
using Northwind.Application.Constants;
using Northwind.Application.Extensions;
using Northwind.Application.Enums;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin},{UserRoles.Customer}")]
    public class OrdersController : Controller
    {
        private static SortBy? Sort;
        private static bool Desc = false;

        private const int pageSize = 10;

        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly ISelectListFiller _selectListFiller;

        public OrdersController(IRepository<Order> orderRepository, IRepository<Customer> customerRepository, IRepository<Product> productRepository,
            ISelectListFiller selectListFiller, IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _selectListFiller = selectListFiller;
        }
        
        public async Task<IActionResult> Index(string? customerId, int page = 1, string? sortBy = null)
        {
            if (!(User.IsInRole(UserRoles.Owner) || User.IsInRole(UserRoles.Admin)) && (customerId == null || customerId != this.HttpContext.Session.GetString(SessionValues.CustomerId)))
            {
                return Redirect("Identity/Account/AccessDenied");
            }

            var orders = await _orderRepository.GetListForAsync(customerId);
            var orderDataModels = _mapper.Map<IEnumerable<OrderIndexDataModel>>(orders);

            if (sortBy != null)
            {
                SortBy? sort = Enum.TryParse(sortBy, out SortBy sortByValue) ? sortByValue : null;

                Desc = Sort == sort ? !Desc : Desc;
                Sort = sort;
            }

            if (Sort != null)
            {
                orderDataModels = orderDataModels.SortSequence(Sort, Desc);
            }
            
            orderDataModels = orderDataModels.Skip((page - 1) * pageSize).Take(pageSize);

            var pageModel = new OrderPageModel(orders.Count(), page, pageSize, customerId);
            var orderIndexModel = new OrderIndexModel(orderDataModels, pageModel);

            if (User.IsInRole(UserRoles.Admin))
            {
                _selectListFiller.FillSelectLists(orderIndexModel, customerId: customerId);
            }

            if (!customerId.IsNullOrEmpty())
            { 
                ViewBag.PreviousPage = Url.ActionLink("Details", "Customers", new { id = customerId });
            }

            ViewBag.CustomerId = customerId;
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

        public IActionResult Create(string? customerId)
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Orders", new { customerId = customerId });

            var orderCreateModel = new OrderCreateModel();
            orderCreateModel.CustomerId = customerId;

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

            _selectListFiller.FillSelectLists(orderCreateModel, employeeId: orderCreateModel.EmployeeId, shipperId: orderCreateModel.ShipperId, 
                customerId: orderCreateModel.CustomerId);

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

            _selectListFiller.FillSelectLists(orderEditModel, employeeId: orderEditModel.EmployeeId, shipperId: orderEditModel.ShipperId, 
                customerId: orderEditModel.CustomerId);

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

            _selectListFiller.FillSelectLists(orderEditModel, employeeId: orderEditModel.EmployeeId, shipperId: orderEditModel.ShipperId, 
                customerId: orderEditModel.CustomerId);

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
            var customerId = (await _orderRepository.GetAsync(ids.FirstOrDefault()))?.CustomerId;

            await _orderRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index), new { fkId = customerId });
        }

        public async Task<IActionResult> Confirm(int? orderId)
        {
            if (orderId == null && !await OrderExists(orderId))
            { 
                return NotFound($"Order {orderId} not found");
            }

            var order = await _orderRepository.GetAsync(orderId);

            if (order?.CustomerId != HttpContext.Session.GetString(SessionValues.CustomerId))
            {
                return Redirect("Identity/Account/AccessDenied");
            }

            var orderDetails = order!.OrderDetails;

            foreach (var orderDetail in orderDetails)
            {
                var product = await _productRepository.GetAsync(orderDetail.ProductId);

                if(product != null && product.UnitsInStock != null)
                {
                    product.UnitsInStock = (short?)(product.UnitsInStock - orderDetail.Quantity);
                    product.UnitsOnOrder = (short?)(product.UnitsOnOrder ?? 0 + orderDetail.Quantity);

                    if (product.UnitsInStock < 0)
                    {
                        return View("../ExceptionView", "Units in stock has negative value");
                    }

                    await _productRepository.UpdateAsync(product);
                }
            }

            order!.OrderDate = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            this.HttpContext.Session.Remove(SessionValues.OrderId);
            this.HttpContext.Session.Remove(SessionValues.OrderStatus);

            ViewBag.Link = Url.ActionLink("Index", "OrderDetails", new { orderId = orderId });
            return View();
        }

        public async Task<IActionResult> Cancel(int? id)
        {
            var order = await _orderRepository.GetAsync(id);
            var customerId = this.HttpContext.Session.GetString(SessionValues.CustomerId);

            if (!User.IsInRole(UserRoles.Admin) && order?.CustomerId != customerId)
            {
                return Redirect("Identity/Account/AccessDenied");
            }

            await _orderRepository.DeleteAsync(id);

            this.HttpContext.Session.Remove(SessionValues.OrderId);
            this.HttpContext.Session.Remove(SessionValues.OrderStatus);

            return RedirectToAction(nameof(Index), new { customerId = customerId });
        }

        private async Task<bool> OrderExists(int? id)
        {
            return (await _orderRepository.GetAsync(id)) != null;
        }
    }
}
