using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models;
using Northwind.Application.Models.Product;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    public class ProductController : Controller
    {
        private const int pageSize = 7;

        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Supplier> _supplierRepository;

        public ProductController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, 
            IRepository<Supplier> supplierRepository, IMapper mapper)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<IActionResult> Index(int fkId = 0, int page = 1)
        {
            var allProducts = await _productRepository.GetListForAsync(fkId);
            var products = allProducts.Skip((page - 1) * pageSize).Take(pageSize);
            var productDataModels = _mapper.Map<IEnumerable<ProductIndexDataModel>>(products);

            var pageModel = new PageViewModel(allProducts.Count(), page, pageSize, fkId);
            var productIndexModel = new ProductIndexModel(productDataModels, pageModel);

            ViewBag.PreviousPage = Url.ActionLink("Details", "Category", new { id = fkId });
            ViewBag.Id = fkId;
            var category = await _categoryRepository.GetAsync(fkId);
            ViewBag.CategoryName = category != null ? category.CategoryName : "";

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

            product.Supplier = await _supplierRepository.GetAsync(product.SupplierId);
            product.Category = await _categoryRepository.GetAsync(product.CategoryId);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Product", new { id = product.CategoryId });

            return View(_mapper.Map<ProductDetailsModel>(product));
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create(int id)
        {
            ViewBag.PreviousPage = Url.ActionLink("Index", "Product", new { id });

            var productCreateModel = new ProductCreateModel();
            productCreateModel.CategoryIdList = GetCategoryIdSelectList(id);
            productCreateModel.SupplierIdList = GetSupplierIdSelectList();

            return View(productCreateModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateModel productCreateModel)
        {
            if (ModelState.IsValid)
            {
                var product = _mapper.Map<Product>(productCreateModel);
                await _productRepository.CreateAsync(product);

                return RedirectToAction(nameof(Index), new { id = product.CategoryId });
            }

            ViewBag.PreviousPage = Url.ActionLink("Index", "Product", new { id = productCreateModel.CategoryId });
            productCreateModel.CategoryIdList = GetCategoryIdSelectList(productCreateModel.CategoryId);
            productCreateModel.SupplierIdList = GetSupplierIdSelectList(productCreateModel.SupplierId);

            return View(productCreateModel);
        }

        [Authorize(Roles = "admin")]
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

            productEditModel.CategoryList = GetCategoryIdSelectList(productEditModel.Category);
            productEditModel.SupplierList = GetSupplierIdSelectList(productEditModel.Supplier);

            return View(productEditModel);
        }

        [Authorize(Roles = "admin")]
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

                return RedirectToAction(nameof(Index), new { id = productEditModel.Category });
            }

            productEditModel.CategoryList = GetCategoryIdSelectList(productEditModel.Category);
            productEditModel.SupplierList = GetSupplierIdSelectList(productEditModel.Supplier);

            return View(productEditModel);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int?[] ids)
        {
            var products = await _productRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Product", new { id = products.Any() ? products.First().CategoryId : 0 });

            return View(_mapper.Map<IEnumerable<ProductDeleteModel>>(products));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int?[] ids)
        {
            var categoryId = await _productRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index), new { id = categoryId });
        }

        private async Task<bool> ProductExists(int id)
        {
            return (await _productRepository.GetAsync(id)) != null;
        }

        private SelectList GetCategoryIdSelectList(int? categoryId = null)
        {
            var categories = _categoryRepository.GetListAsync().Result;
            var dictionary = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);
            dictionary.Add(0, "");

            var selectList = new SelectList(dictionary, "Key", "Value", dictionary);

            SelectListItem selectedItem = null;

            if (categoryId != null)
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == categoryId.ToString());
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

        private SelectList GetSupplierIdSelectList(int? supplierId = null)
        {
            var suppliers = _supplierRepository.GetListAsync().Result;
            var dictionary = suppliers.ToDictionary(c => c.SupplierId, c => c.CompanyName);
            dictionary.Add(0, "");

            var selectList = new SelectList(dictionary, "Key", "Value", dictionary);

            SelectListItem selectedItem = null;

            if (supplierId != null)
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == supplierId.ToString());
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
