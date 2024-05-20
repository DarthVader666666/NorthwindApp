using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Constants;
using Northwind.Application.Enums;
using Northwind.Application.Extensions;
using Northwind.Application.Models.Order;
using Northwind.Application.Models.PageModels;
using Northwind.Application.Models.Supplier;
using Northwind.Application.Services;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
    public class SuppliersController : Controller
    {
        private static SortBy? Sort;
        private static bool Desc = false;

        private const int pageSize = 10;

        private readonly IMapper _mapper;
        private readonly ISelectListFiller _selectListFiller;
        private readonly IRepository<Supplier> _supplierRepository;

        public SuppliersController(IRepository<Supplier> supplierRepository, ISelectListFiller selectListFiller, IMapper mapper)
        {
            _mapper = mapper;
            _selectListFiller = selectListFiller;
            _supplierRepository = supplierRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string? sortBy = null)
        {
            if (!(User.IsInRole(UserRoles.Owner) || User.IsInRole(UserRoles.Admin)))
            {
                return Redirect("Identity/Account/AccessDenied");
            }

            var suppliers = await _supplierRepository.GetListAsync();
            var supplierDataModels = _mapper.Map<IEnumerable<SupplierIndexDataModel>>(suppliers);

            if (sortBy != null)
            {
                SortBy? sort = Enum.TryParse(sortBy, out SortBy sortByValue) ? sortByValue : null;
                Desc = Sort == sort ? !Desc : Desc;
                Sort = sort;
            }

            if (Sort != null)
            {
                supplierDataModels = supplierDataModels.SortSequence(Sort, Desc);
            }

            supplierDataModels = supplierDataModels.Skip((page - 1) * pageSize).Take(pageSize);

            var pageModel = new SupplierPageModel(suppliers.Count(), page, pageSize);
            var supplierIndexModel = new SupplierIndexModel(supplierDataModels, pageModel);

            return View(supplierIndexModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _supplierRepository.GetAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            var supplierDetailsModel = _mapper.Map<SupplierDetailsModel>(supplier);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Suppliers");

            return View(supplierDetailsModel);
        }

        public IActionResult Create()
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Suppliers");

            var supplierCreateModel = new SupplierCreateModel();

            return View(supplierCreateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierCreateModel supplierCreateModel)
        {
            if (ModelState.IsValid)
            {
                var supplier = _mapper.Map<Supplier>(supplierCreateModel);
                await _supplierRepository.CreateAsync(supplier);

                return RedirectToAction(nameof(Index));
            }

            return View(supplierCreateModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplierEditModel = _mapper.Map<SupplierEditModel>(await _supplierRepository.GetAsync(id));

            if (supplierEditModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(supplierEditModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierEditModel supplierEditModel)
        {
            if (id != supplierEditModel.SupplierId)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var supplier = _mapper.Map<Supplier>(supplierEditModel);
                    await _supplierRepository.UpdateAsync(supplier);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SupplierExists(supplierEditModel.SupplierId))
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

            return View(supplierEditModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var suppliers = await _supplierRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Suppliers");

            return View(_mapper.Map<IEnumerable<SupplierIndexDataModel>>(suppliers));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _supplierRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SupplierExists(int id)
        {
            return (await _supplierRepository.GetAsync(id)) != null;
        }
    }
}
