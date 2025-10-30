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
using Northwind.Application.Models.Product;
using Northwind.Bll.Services;
using Northwind.Bll.Services.Extensions;
using System.Globalization;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin},{UserRoles.Customer},{UserRoles.Seller}")]
    public class OrdersController : Controller
    {
        private static SortBy? Sort;
        private static SelectListName? selectListName = SelectListName.CustomerList;
        private static bool Desc = false;

        private const int pageSize = 10;

        private readonly IMapper _mapper;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Seller> _sellerRepository;
        private readonly ISelectListFiller _selectListFiller;

        public OrdersController(IRepository<Order> orderRepository, IRepository<Customer> customerRepository, IRepository<Product> productRepository,
            IRepository<Seller> sellerRepository, ISelectListFiller selectListFiller, IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _selectListFiller = selectListFiller;
            _sellerRepository = sellerRepository;
        }
        
        public async Task<IActionResult> Index(string? customerId, int? sellerId, int page = 1, string? sortBy = null)
        {
            if (!(User.IsInRole(UserRoles.Owner) || User.IsInRole(UserRoles.Admin) || User.IsInRole(UserRoles.Seller)) && (customerId == null || customerId != this.HttpContext.Session.GetString(SessionValues.CustomerId)))
            {
                return Redirect("Identity/Account/AccessDenied");
            }

            var foreignKeys = Tuple.Create(customerId == "0" ? "" : customerId, sellerId?.ToString());
            var orders = await _orderRepository.GetListForAsync(foreignKeys);
            var orderDataModels = _mapper.Map<IEnumerable<OrderIndexDataModel>>(orders);
            var columnWidths = orderDataModels.GetColumnWidths();

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

            var pageModel = new OrderPageModel(orders.Count(), page, pageSize, customerId, sellerId);
            var orderIndexModel = new OrderIndexModel(orderDataModels, pageModel);

            selectListName = (customerId, sellerId) switch
            {
                ("0", null) => SelectListName.CustomerList,
                (null, >= 0) => SelectListName.SellerList,
                (null, null) => page > 1 ? selectListName : SelectListName.CustomerList,
                _ => SelectListName.CustomerList
            };

            if (selectListName == SelectListName.CustomerList)
            {
                customerId = customerId == "0" ? null : customerId;
                ViewBag.PreviousPage = Url.ActionLink("Details", "Customers", new { id = customerId });
                orderIndexModel.CustomerList = _selectListFiller.GetCustomerIdSelectList(customerId, all: true);
                ViewBag.ForeignKeyValue = customerId;
                ViewBag.ForeignKeyName = "customerId";
                ViewBag.CompanyName = (await _customerRepository.GetAsync(customerId))?.CompanyName ?? "";
            }

            if (selectListName == SelectListName.SellerList)
            {
                ViewBag.PreviousPage = Url.ActionLink("Details", "Sellers", new { id = sellerId });
                orderIndexModel.SellerList = _selectListFiller.GetSellerIdSelectList(sellerId, all: true);
                ViewBag.ForeignKeyValue = sellerId;
                ViewBag.ForeignKeyName = "sellerId";
                var emloyee = await _sellerRepository.GetAsync(sellerId);
                ViewBag.SellerName = emloyee != null ? $"{emloyee?.FirstName} {emloyee?.LastName}" : null;
            }

            ViewBag.PageStartNumbering = (page - 1) * pageSize + 1;
            ViewBag.SelectListName = selectListName.ToString();
            ViewBag.ColumnWidths = columnWidths;

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

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        public IActionResult Create(string? customerId)
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Orders", new { customerId = customerId });

            var orderCreateModel = new OrderCreateModel();
            orderCreateModel.CustomerId = customerId;

            _selectListFiller.FillSelectLists(orderCreateModel, customerId: customerId);

            return View(orderCreateModel);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
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

            _selectListFiller.FillSelectLists(orderCreateModel, sellerId: orderCreateModel.SellerId, shipperId: orderCreateModel.ShipperId, 
                customerId: orderCreateModel.CustomerId);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Orders");
            return View(orderCreateModel);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
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

            _selectListFiller.FillSelectLists(orderEditModel, sellerId: orderEditModel.SellerId, shipperId: orderEditModel.ShipperId, 
                customerId: orderEditModel.CustomerId);

            return View(orderEditModel);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
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

            _selectListFiller.FillSelectLists(orderEditModel, sellerId: orderEditModel.SellerId, shipperId: orderEditModel.ShipperId, 
                customerId: orderEditModel.CustomerId);

            return View(orderEditModel);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        [HttpGet]
        public async Task<ActionResult> Update(int? orderId, int? sellerId)
        {
            if (orderId == null || sellerId == null)
            {
                return BadRequest();
            }

            if (sellerId == 0)
            {
                sellerId = null;
            }

            var order = await _orderRepository.GetAsync(orderId);

            if (order == null)
            { 
                return NotFound();
            }

            order.SellerId = sellerId;
            await _orderRepository.UpdateAsync(order);

            return Ok();
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var orders = await _orderRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Orders");

            return View(_mapper.Map<IEnumerable<OrderIndexDataModel>>(orders));
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            var customerId = (await _orderRepository.GetAsync(ids.FirstOrDefault()))?.CustomerId;

            await _orderRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index), new { customerId = customerId });
        }

        [Authorize(Roles = $"{UserRoles.Customer}")]
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

                    product.Category = null;
                    product.Supplier = null;
                    await _productRepository.UpdateAsync(product);
                }
            }

            order!.OrderDate = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            this.HttpContext.Session.Remove(SessionValues.OrderId);
            this.HttpContext.Session.Remove(SessionValues.OrderStatus);

            return View();
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner},{UserRoles.Customer}")]
        public async Task<IActionResult> Cancel(int? orderId, int? categoryId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            var customerId = this.HttpContext.Session.GetString(SessionValues.CustomerId);

            if (!User.IsInRole(UserRoles.Admin) && order?.CustomerId != customerId)
            {
                return Redirect("Identity/Account/AccessDenied");
            }

            await _orderRepository.DeleteAsync(orderId);

            this.HttpContext.Session.Remove(SessionValues.OrderId);
            this.HttpContext.Session.Remove(SessionValues.OrderStatus);

            return RedirectToAction("Index", "Products", new { categoryId = categoryId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        public async Task<IActionResult> Workflow(string? sortBy = null)
        {
            var orders = (await _orderRepository.GetListAsync()).Where(x => x != null && (x.RequiredDate == null || x.ShippedDate == null));
            var orderWorkflowModels = _mapper.Map<IEnumerable<OrderWorkflowModel>>(orders);

            foreach (var order in orderWorkflowModels)
            {
                order.SellerList = _selectListFiller.GetSellerIdSelectList(sellerId: order.SellerId);
            }
            
            var columnWidths = orderWorkflowModels.GetColumnWidths();

            if (sortBy != null)
            {
                SortBy? sort = Enum.TryParse(sortBy, out SortBy sortByValue) ? sortByValue : null;

                Desc = Sort == sort ? !Desc : Desc;
                Sort = sort;
            }

            if (Sort != null)
            {
                orderWorkflowModels = orderWorkflowModels.SortSequence(Sort, Desc);
            }

            ViewBag.ColumnWidths = columnWidths;

            return View(orderWorkflowModels);
        }

        private async Task<bool> OrderExists(int? id)
        {
            return (await _orderRepository.GetAsync(id)) != null;
        }
    }
}
