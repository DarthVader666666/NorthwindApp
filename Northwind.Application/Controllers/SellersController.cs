using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Seller;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Northwind.Application.Constants;
using Northwind.Application.Services;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
    public class SellersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Seller> _sellerRepository;
        private readonly ISelectListFiller _selectListFiller;

        public SellersController(IRepository<Seller> sellerRepository, ISelectListFiller selectListFiller, IMapper mapper)
        {
            _mapper = mapper;
            _selectListFiller = selectListFiller;
            _sellerRepository = sellerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var sellers = await _sellerRepository.GetListAsync();
            var sellerModels = _mapper.Map<IEnumerable<SellerIndexModel>>(sellers);

            return View(sellerModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _sellerRepository.GetAsync(id);

            if (seller == null)
            {
                return NotFound();
            }

            var sellerDetailsModel = _mapper.Map<SellerDetailsModel>(seller);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Sellers");

            return View(sellerDetailsModel);
        }

        public IActionResult Create()
        {
            ViewBag.ReportsTo = GetReportsToSelectList();
            ViewBag.PreviousPage = Url.ActionLink("Index", "Sellers");

            var sellerCreateModel = new SellerCreateModel();
            sellerCreateModel.ReportsToList = GetReportsToSelectList();

            return View(sellerCreateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SellerCreateModel sellerCreateModel)
        {
            if (ModelState.IsValid)
            {
                var seller = _mapper.Map<Seller>(sellerCreateModel);
                await _sellerRepository.CreateAsync(seller);

                return RedirectToAction(nameof(Index));
            }

            sellerCreateModel.ReportsToList = GetReportsToSelectList();

            return View(sellerCreateModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerEditModel = _mapper.Map<SellerEditModel>(await _sellerRepository.GetAsync(id));

            if (sellerEditModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            sellerEditModel.ReportsToList = GetReportsToSelectList(id: id, reportsTo: sellerEditModel.ReportsTo ?? 0);

            return View(sellerEditModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SellerEditModel sellerEditModel)
        {
            if (id != sellerEditModel.SellerId)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var seller = _mapper.Map<Seller>(sellerEditModel);
                    await _sellerRepository.UpdateAsync(seller);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SellerExists(sellerEditModel.SellerId))
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

            sellerEditModel.ReportsToList = GetReportsToSelectList(id: id, reportsTo:sellerEditModel.ReportsTo);
            return View(sellerEditModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var sellers = await _sellerRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Sellers");

            return View(_mapper.Map<IEnumerable<SellerIndexModel>>(sellers));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _sellerRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SellerExists(int id)
        {
            return (await _sellerRepository.GetAsync(id)) != null;
        }

        private SelectList? GetReportsToSelectList(int? id = null, int? reportsTo = null)
        {
            var list = _sellerRepository.GetListAsync().Result;
            var dictionary = list.Except(list.Where(e => e.SellerId == id)).ToDictionary(e => e.SellerId, e => e.FirstName + " " + e.LastName);

            return _selectListFiller.GetSelectList(dictionary, reportsTo);
        }
    }
}
