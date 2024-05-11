using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Application.Models.OrderDetail;
using Northwind.Application.Models;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Northwind.Application.Models.PageModels;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles ="admin, customer")]
    public class OrderDetailsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Order> _orderRepository;
        private const int pageSize = 6;

        public OrderDetailsController(IRepository<OrderDetail> orderDetailRepository, IRepository<Product> productRepository, 
            IRepository<Order> orderRepository, IMapper mapper)
        {
            _mapper = mapper;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        //public async Task<IActionResult> Index(int fkId = 0, int page = 1)
        //{
        //    var allOrderDetails = await _orderDetailRepository.GetListForAsync(fkId);
        //    var orderDetails = allOrderDetails.Skip((page - 1) * pageSize).Take(pageSize);
        //    var orderDetailDataModels = _mapper.Map<IEnumerable<OrderDetailIndexDataModel>>(orderDetails);

        //    var pageModel = new PageViewModel(allOrderDetails.Count(), page, pageSize, fkId);
        //    var orderDetailIndexModel = new OrderDetailIndexModel(orderDetailDataModels, pageModel);

        //    if (fkId > 0)
        //    {
        //        ViewBag.PreviousPage = Url.ActionLink("Details", "Order", new { id = fkId });
        //    }

        //    if (!orderDetails.IsNullOrEmpty())
        //    {
        //        var customer = await _customerRepository.GetAsync(orderDetails.First()!.Order.CustomerId);
        //        ViewBag.CompanyName = customer == null ? "" : customer.CompanyName;
        //    }

        //    ViewBag.Id = fkId;

        //    return View(orderDetailIndexModel);
        //}

        public async Task<IActionResult> Index(int orderId = 0, int productId = 0, int page = 1)
        {
            var primaryKeys = $"{orderId} {productId}";
            var allOrderDetails = await _orderDetailRepository.GetListForAsync(primaryKeys);
            var orderDetails = allOrderDetails.Skip((page - 1) * pageSize).Take(pageSize);
            var orderDetailDataModels = _mapper.Map<IEnumerable<OrderDetailIndexDataModel>>(orderDetails);

            var pageModel = new OrderDetailsPageModel(allOrderDetails.Count(), page, pageSize, orderId, productId);
            var orderDetailIndexModel = new OrderDetailIndexModel(orderDetailDataModels, pageModel);

            if (orderId > 0)
            {
                ViewBag.PreviousPage = Url.ActionLink("Details", "Orders", new { id = orderId });

                if (!orderDetailDataModels.IsNullOrEmpty())
                {
                    ViewBag.ProductOrCompanyName = orderDetailDataModels.First().CompanyName;
                }
            }

            if (productId > 0)
            {
                ViewBag.PreviousPage = Url.ActionLink("Details", "Products", new { id = productId });
                ViewBag.ProductOrCompanyName = orderDetailDataModels.First().ProductName;
            }

            return View(orderDetailIndexModel);
        }

        public async Task<IActionResult> Details(object? ids)
        {
            if (ids == null)
            {
                return NotFound();
            }

            var orderDetail = await _orderDetailRepository.GetAsync(ids);

            if (orderDetail == null)
            {
                return NotFound();
            }

            ViewBag.PreviousPage = Url.ActionLink("Index", "OrderDetails", new { orderId = orderDetail.OrderId });

            return View(orderDetail);
        }

        public async Task<IActionResult> Create(int? productId = null)
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "OrderDetails");

            if(productId != null) 
            {
                var product = await _productRepository.GetAsync(productId);

                if (product == null) 
                {
                    return NotFound("Product not found");
                }

                ViewBag.ProductName = product.ProductName;
                var orderDetailCreateModel = new OrderDetailCreateModel { ProductId = productId, UnitPrice = product.UnitPrice };
                return View(orderDetailCreateModel);
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetailCreateModel orderDetailCreateModel)
        {
            if (ModelState.IsValid)
            {
                var customerId = this.HttpContext.Session.GetString(SessionValues.CustomerId);

                if (!customerId.IsNullOrEmpty() && this.HttpContext.Session.GetString(SessionValues.OrderStatus) != SessionValues.InProgress)
                {
                    this.HttpContext.Session.SetString(SessionValues.OrderStatus, SessionValues.InProgress);
                    var order = await _orderRepository.CreateAsync(new Order { CustomerId = customerId, OrderDate = DateTime.UtcNow });

                    if (order != null)
                    {
                        this.HttpContext.Session.SetInt32(SessionValues.OrderId, order.OrderId);
                    }
                }

                var orderId = this.HttpContext.Session.GetInt32(SessionValues.OrderId);

                if (orderId == null)
                {
                    this.HttpContext.Session.SetString(SessionValues.OrderStatus, SessionValues.InProgress);
                    return NotFound("Order not found");
                }

                orderDetailCreateModel.OrderId = orderId;

                var orderDetail = _mapper.Map<OrderDetail>(orderDetailCreateModel);

                if (await _orderDetailRepository.GetAsync((orderDetail.OrderId, orderDetail.ProductId)) == null)
                {
                    await _orderDetailRepository.CreateAsync(orderDetail);
                }
                else 
                {
                    await _orderDetailRepository.UpdateAsync(orderDetail);
                }

                return RedirectToAction("Details", "Products", new { id = orderDetailCreateModel.ProductId });
            }

            return View(orderDetailCreateModel);
        }

        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderDetailEditModel = _mapper.Map<OrderDetailEditModel>(await _orderDetailRepository.GetAsync(id));

        //    if (orderDetailEditModel == null)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(orderDetailEditModel);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, OrderDetailEditModel orderDetailEditModel)
        //{
        //    if (id != orderDetailEditModel.OrderId)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var orderDetail = _mapper.Map<OrderDetail>(orderDetailEditModel);
        //            await _orderDetailRepository.UpdateAsync(orderDetail);
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!await OrderDetailExists(orderDetailEditModel.OrderId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(orderDetailEditModel);
        //}

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            var orderDetails = await _orderDetailRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "OrderDetails");

            return View(_mapper.Map<IEnumerable<OrderDetailIndexDataModel>>(orderDetails));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string[] ids)
        {
            await _orderDetailRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index), new { fkId = ids[0].Split(' ')[0] });
        }
    }
}
