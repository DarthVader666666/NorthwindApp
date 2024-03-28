using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Category;
using Northwind.Bll.Enums;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(IRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetList();
            var categoryModels = _mapper.Map<IEnumerable<CategoryIndexModel>>(categories);

            return View(categoryModels);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryRepository.Get(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateModel categoryCreateModel)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(categoryCreateModel);
                await _categoryRepository.Create(category);

                return RedirectToAction(nameof(Index));
            }

            return View(categoryCreateModel);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryEditModel = _mapper.Map<CategoryEditModel>(await _categoryRepository.Get(id));

            if (categoryEditModel == null)
            {
                return NotFound();
            }

            return View(categoryEditModel);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryEditModel categoryEditModel)
        {
            if (id != categoryEditModel.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = _mapper.Map<Category>(categoryEditModel);

                    await _categoryRepository.Update(category);
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

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.Delete(id);
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExists(int id)
        {
            return (await _categoryRepository.Get(id)) != null;
        }
    }
}
