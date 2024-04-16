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

        public ProductsController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        protected string? ViewPath { get; set; } = "Views/Products/";

        // GET: Products
        public async Task<IActionResult> Index(int categoryId)
        {
            var products = _productRepository.GetListFor(categoryId);

            if (!products.Any())
            {
                Redirect("Categories/Index/");
            }

            var productModels = _mapper.Map<IEnumerable<ProductIndexModel>>(products);

            var productsForCategory = new ProductsForCategoryModel() 
            { 
                Products = productModels,
                CategoryName = (await _categoryRepository.Get(categoryId)).CategoryName 
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

            var product = await _productRepository.Get(id);

            if (product == null)
            {
                return NotFound();
            }


            return View($"{ViewPath}Details.cshtml", product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.CategoryId = GetCategoryIdSelectList();
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
                await _productRepository.Create(product);

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

            var productEditModel = _mapper.Map<ProductEditModel>(await _productRepository.Get(id));

            if (productEditModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

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
                    await _productRepository.Update(product);
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

                var product = await _productRepository.Get(id);

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
            var categoryId = await _productRepository.DeleteSeveral(ids);

            return RedirectToAction(nameof(Index), new { categoryId });
        }

        private async Task<bool> ProductExists(int id)
        {
            return (await _productRepository.Get(id)) != null;
        }

        private SelectList GetCategoryIdSelectList(int? id = null)
        {
            var list = _categoryRepository.GetList();
            var dictionary = list.Except(list.Where(e => e.CategoryId == id)).ToDictionary(e => e.CategoryId, e => e.CategoryName);

            dictionary.Add(0, "");
            return new SelectList(dictionary, "Key", "Value");
        }
    }
}
