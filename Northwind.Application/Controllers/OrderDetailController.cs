using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Application.Models.OrderDetail;
using Northwind.Application.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace Northwind.Application.Controllers
{
    //[Authorize(Roles ="admin, customer")]
    public class OrderDetailController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<Customer> _customerRepository;
        private const int pageSize = 6;

        public OrderDetailController(IRepository<OrderDetail> orderDetailRepository, IRepository<Customer> customerRepository, IMapper mapper)
        {
            _mapper = mapper;
            _orderDetailRepository = orderDetailRepository;
            _customerRepository = customerRepository;
        }

        public async Task<IActionResult> Index(int fkId = 0, int page = 1)
        {
            var allOrderDetails = await _orderDetailRepository.GetListForAsync(fkId);
            var orderDetails = allOrderDetails.Skip((page - 1) * pageSize).Take(pageSize);
            var orderDetailDataModels = _mapper.Map<IEnumerable<OrderDetailIndexDataModel>>(orderDetails);

            var pageModel = new PageViewModel(allOrderDetails.Count(), page, pageSize, fkId);
            var orderDetailIndexModel = new OrderDetailIndexModel(orderDetailDataModels, pageModel);

            if (fkId > 0)
            {
                ViewBag.PreviousPage = Url.ActionLink("Details", "Order", new { id = fkId });
            }

            if (!orderDetails.IsNullOrEmpty())
            {
                var customer = await _customerRepository.GetAsync(orderDetails.First().Order.CustomerId);
                ViewBag.CompanyName = customer == null ? "" : customer.CompanyName;
            }

            ViewBag.Id = fkId;

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

            ViewBag.PreviousPage = Url.ActionLink("Index", "OrderDetail", new { fkId = orderDetail.OrderId });

            return View(orderDetail);
        }

        public IActionResult Create(string fkId)
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "OrderDetail", new { fkId = fkId });

            var orderDetailCreateModel = new OrderDetailCreateModel();

            return View(orderDetailCreateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetailCreateModel orderDetailCreateModel)
        {
            if (ModelState.IsValid)
            {
                var orderDetail = _mapper.Map<OrderDetail>(orderDetailCreateModel);
                await _orderDetailRepository.CreateAsync(orderDetail);

                return RedirectToAction(nameof(Index), new { fkId = orderDetail.OrderId });
            }

            return View(orderDetailCreateModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetailEditModel = _mapper.Map<OrderDetailEditModel>(await _orderDetailRepository.GetAsync(id));

            if (orderDetailEditModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(orderDetailEditModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderDetailEditModel orderDetailEditModel)
        {
            if (id != orderDetailEditModel.OrderId)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var orderDetail = _mapper.Map<OrderDetail>(orderDetailEditModel);
                    await _orderDetailRepository.UpdateAsync(orderDetail);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OrderDetailExists(orderDetailEditModel.OrderId))
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

            return View(orderDetailEditModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var orderDetails = await _orderDetailRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "OrderDetail");

            return View(_mapper.Map<IEnumerable<OrderDetailIndexDataModel>>(orderDetails));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _orderDetailRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> OrderDetailExists(int id)
        {
            return (await _orderDetailRepository.GetAsync(id)) != null;
        }
    }
}
