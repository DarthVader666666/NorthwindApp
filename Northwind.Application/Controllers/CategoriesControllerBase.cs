using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Category;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    public abstract class CategoriesControllerBase : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        protected CategoriesControllerBase(IRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        protected string ViewPath { get; set; } = "Views/Categories/";

        // GET: Categories
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetList();
            var categoryModels = _mapper.Map<IEnumerable<CategoryIndexModel>>(categories);

            return View($"{ViewPath}Index.cshtml", categoryModels);
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

            return View($"{ViewPath}Details.cshtml", category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View($"{ViewPath}Create.cshtml");
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

            return View($"{ViewPath}Create.cshtml", categoryCreateModel);
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
                RedirectToAction(nameof(Index));
            }

            return View($"{ViewPath}Edit.cshtml", categoryEditModel);
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

            return View($"{ViewPath}Edit.cshtml", categoryEditModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var categories = new List<Category>();

            foreach (var id in ids)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _categoryRepository.Get(id);

                if (employee == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                categories.Add(employee);
            }

            return View($"{ViewPath}Delete.cshtml", _mapper.Map<IEnumerable<CategoryIndexModel>>(categories));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _categoryRepository.DeleteSeveral(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExists(int id)
        {
            return (await _categoryRepository.Get(id)) != null;
        }
    }
}
