using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Category;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(IRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepository.GetList();
            var categoryModels = _mapper.Map<IEnumerable<CategoryIndexModel>>(categories);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Category");

            return View(categoryModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryRepository.GetAsync(id);

            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PreviousPage = Url.ActionLink("Index", "Category");

            return View(category);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateModel categoryCreateModel)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(categoryCreateModel);
                await _categoryRepository.CreateAsync(category);

                return RedirectToAction(nameof(Index));
            }

            return View(categoryCreateModel);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryEditModel = _mapper.Map<CategoryEditModel>(await _categoryRepository.GetAsync(id));

            if (categoryEditModel == null)
            {
                RedirectToAction(nameof(Index));
            }

            return View(categoryEditModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryEditModel categoryEditModel)
        {
            if (id != categoryEditModel.CategoryId)
            {
                RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = _mapper.Map<Category>(categoryEditModel);

                    await _categoryRepository.UpdateAsync(category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CategoryExists(categoryEditModel.CategoryId))
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

            return View(categoryEditModel);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int?[] ids)
        {
            var categories = new List<Category>();

            foreach (var id in ids)
            {
                if (id == null)
                {
                    continue;
                }

                var employee = await _categoryRepository.GetAsync(id);

                if (employee == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                categories.Add(employee);
            }

            ViewBag.PreviousPage = Url.ActionLink("Index", "Category");

            return View(_mapper.Map<IEnumerable<CategoryIndexModel>>(categories));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _categoryRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExists(int id)
        {
            return (await _categoryRepository.GetAsync(id)) != null;
        }
    }
}
