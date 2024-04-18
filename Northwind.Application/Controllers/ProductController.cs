using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Product;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    public class ProductController : Controller
    {
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

        public async Task<IActionResult> Index(int categoryId = 0)
        {
            var products = _productRepository.GetListFor(categoryId);
            var productModels = _mapper.Map<IEnumerable<ProductIndexModel>>(products);
            ViewBag.PreviousPage = Url.ActionLink("Details", "Category", new { id = categoryId });

            return View(productModels);
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

            return View(product);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryIds = GetCategoryIdSelectList();
            ViewBag.SupplierIds = GetSupplierIdSelectList();

            return View();
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

                return RedirectToAction(nameof(Index), new { categoryId = product.CategoryId });
            }

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

            ViewBag.CategoryIds = GetCategoryIdSelectList(productEditModel.CategoryId);
            ViewBag.SupplierIds = GetSupplierIdSelectList(productEditModel.SupplierId);

            return View(productEditModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditModel productEditModel)
        {
            if (id != productEditModel.ProductId)
            {
                return NotFound();
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

                return RedirectToAction(nameof(Index));
            }

            return View(productEditModel);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var productforCategoryModel = new ProductForCategoryModel();
            var products = new List<ProductIndexModel>();

            foreach (var id in ids)
            {
                var product = await _productRepository.GetAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                products.Add(_mapper.Map<ProductIndexModel>(product));
            }

            productforCategoryModel.Products = products;

            return View(productforCategoryModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            var categoryId = await _productRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index), new { categoryId });
        }

        private async Task<bool> ProductExists(int id)
        {
            return (await _productRepository.GetAsync(id)) != null;
        }

        private SelectList GetCategoryIdSelectList(int categoryId = 0)
        {
            var categories = _categoryRepository.GetList();
            var dictionary = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);
            dictionary.Add(0, "");

            return new SelectList(dictionary, "Key", "Value", dictionary[categoryId]);
        }

        private SelectList GetSupplierIdSelectList(int supplierId = 0)
        {
            var suppliers = _supplierRepository.GetList();
            var dictionary = suppliers.ToDictionary(c => c.SupplierId, c => c.CompanyName);
            dictionary.Add(0, "");

            return new SelectList(dictionary, "Key", "Value", dictionary[supplierId]);
        }
    }
}
