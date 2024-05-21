using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Constants;
using Northwind.Application.Enums;
using Northwind.Application.Extensions;
using Northwind.Application.Models.PageModels;
using Northwind.Application.Models.Product;
using Northwind.Application.Services;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    public class ProductsController : Controller
    {
        private static SortBy? Sort;
        private static SelectListName? selectListName = SelectListName.CategoryList;
        private static bool Desc = false;

        private const int pageSize = 7;

        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly ISelectListFiller _selectListFiller;

        public ProductsController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IRepository<Supplier> supplierRepository,
            ISelectListFiller selectListFiller, IMapper mapper)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _selectListFiller = selectListFiller;
        }

        public async Task<IActionResult> Index(int? categoryId, int? supplierId, int page = 1, string? sortBy = null)
        {
            var foreignKeys = Tuple.Create(categoryId, supplierId);

            var products = await _productRepository.GetListForAsync(foreignKeys);
            var productDataModels = _mapper.Map<IEnumerable<ProductIndexDataModel>>(products);

            if (sortBy != null)
            {
                SortBy? sort = Enum.TryParse(sortBy, out SortBy sortByValue) ? sortByValue : null;

                Desc = Sort == sort ? !Desc : Desc;
                Sort = sort;
            }

            if (Sort != null)
            {
                productDataModels = productDataModels.SortSequence(Sort, Desc);
            }

            productDataModels = productDataModels.Skip((page - 1) * pageSize).Take(pageSize);            

            var pageModel = new ProductPageModel(products.Count(), page, pageSize, categoryId, supplierId);
            var productIndexModel = new ProductIndexModel(productDataModels, pageModel);

            selectListName = (categoryId, supplierId) switch
            {
                ( >= 0, null ) => SelectListName.CategoryList,
                ( null, >= 0 ) => SelectListName.SupplierList,
                ( null, null ) => page > 1 ? selectListName : SelectListName.CategoryList,
                _ => SelectListName.CategoryList
            };

            if (selectListName == SelectListName.CategoryList)
            {
                ViewBag.PreviousPage = Url.ActionLink("Details", "Categories", new { id = categoryId });
                productIndexModel.CategoryList = _selectListFiller.GetCategoryIdSelectList(categoryId, true);
                ViewBag.ForeignKeyValue = categoryId;
                ViewBag.ForeignKeyName = "categoryId";
                ViewBag.CategoryName = (await _categoryRepository.GetAsync(categoryId))?.CategoryName ?? "";
            }

            if (selectListName == SelectListName.SupplierList)
            {
                ViewBag.PreviousPage = Url.ActionLink("Details", "Suppliers", new { id = supplierId });
                productIndexModel.SupplierList = _selectListFiller.GetSupplierIdSelectList(supplierId, true);
                ViewBag.ForeignKeyValue = supplierId;
                ViewBag.ForeignKeyName = "supplierId";
                ViewBag.SupplierName = (await _supplierRepository.GetAsync(supplierId))?.CompanyName ?? "";
            }

            ViewBag.SelectListName = selectListName.ToString();
            ViewBag.PageStartNumbering = (page - 1) * pageSize + 1;

            return View(productIndexModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.PreviousPage = Url.ActionLink("Index", "Products", new { categoryId = product.CategoryId });
            ViewBag.Id = id;

            return View(_mapper.Map<ProductDetailsModel>(product));
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        public IActionResult Create(int? categoryId, int? supplierId)
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Products", new { categoryId });

            var productCreateModel = new ProductCreateModel();
            _selectListFiller.FillSelectLists(productCreateModel, categoryId: categoryId, supplierId: supplierId);

            return View(productCreateModel);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateModel productCreateModel)
        {
            if (ModelState.IsValid)
            {
                var product = _mapper.Map<Product>(productCreateModel);
                await _productRepository.CreateAsync(product);

                return RedirectToAction(nameof(Index), new { categoryId = product.CategoryId });
            }

            ViewBag.PreviousPage = Url.ActionLink("Index", "Products", new { categoryId = productCreateModel.CategoryId });
            _selectListFiller.FillSelectLists(productCreateModel, categoryId: productCreateModel.CategoryId, supplierId: productCreateModel.SupplierId);

            return View(productCreateModel);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productEditModel = _mapper.Map<ProductEditModel>(await _productRepository.GetAsync(id));

            if (productEditModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _selectListFiller.FillSelectLists(productEditModel, categoryId: productEditModel.CategoryId, supplierId: productEditModel.SupplierId);

            return View(productEditModel);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditModel productEditModel)
        {
            if (id != productEditModel.ProductId)
            {
                RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = _mapper.Map<Product>(productEditModel);
                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(productEditModel.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index), new { categoryId = productEditModel.CategoryId });
            }

            _selectListFiller.FillSelectLists(productEditModel, categoryId: productEditModel.CategoryId, supplierId: productEditModel.SupplierId);

            return View(productEditModel);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var products = await _productRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Products", new { categoryId = products.Any() ? products.First().CategoryId : 0 });

            return View(_mapper.Map<IEnumerable<ProductDeleteModel>>(products));
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Owner}")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _productRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            return (await _productRepository.GetAsync(id)) != null;
        }
    }
}
