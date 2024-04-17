using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Product;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Supplier> _supplierRepository;

        public ProductsController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, 
            IRepository<Supplier> supplierRepository, IMapper mapper)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
        }

        protected string? ViewPath { get; set; } = "Views/Products/";

        // GET: Products
        public async Task<IActionResult> Index(int categoryId)
        {
            var products = _productRepository.GetListFor(categoryId);

            var productModels = _mapper.Map<IEnumerable<ProductIndexModel>>(products);
            var categoryName = await _categoryRepository.GetAsync(categoryId);

            var productsForCategory = new ProductsForCategoryModel() 
            { 
                Products = productModels,
                CategoryName = categoryName == null ? "" : categoryName.CategoryName,
                CategoryId = categoryId
            };

            return View($"{ViewPath}Index.cshtml", productsForCategory);
        }

        // GET: Products/Details/5
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

            return View($"{ViewPath}Details.cshtml", product);
        }

        // GET: Products/Create
        public IActionResult Create(int categoryId = 0)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryIds = GetCategoryIdSelectList();
            ViewBag.SupplierIds = GetSupplierIdSelectList();

            return View($"{ViewPath}Create.cshtml");
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
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

            return View($"{ViewPath}Create.cshtml", productCreateModel);
        }

        // GET: Products/Edit/5
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

            return View($"{ViewPath}Edit.cshtml", productEditModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
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

            return View($"{ViewPath}Edit.cshtml", productEditModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var productforCategoryModel = new ProductsForCategoryModel();
            var products = new List<ProductIndexModel>();

            foreach (var id in ids)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _productRepository.GetAsync(id);

                if (product == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                products.Add(_mapper.Map<ProductIndexModel>(product));
            }

            productforCategoryModel.Products = products;

            return View($"{ViewPath}Delete.cshtml", productforCategoryModel);
        }

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
