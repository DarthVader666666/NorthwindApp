using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Employee;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Northwind.Application.Controllers
{
    public abstract class EmployeesControllerBase : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Employee> _employeeRepository;

        protected EmployeesControllerBase(IRepository<Employee> employeeRepository, IMapper mapper)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;            
        }

        protected string? ViewPath { get; set; } = "Views/Employees/";

        // GET: Employees
        public IActionResult Index()
        {
            var employees = _employeeRepository.GetList();
            var employeeModels = _mapper.Map<IEnumerable<EmployeeIndexModel>>(employees);

            return View($"{ViewPath}Index.cshtml", employeeModels);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.Get(id);

            if (employee == null)
            {
                return NotFound();
            }

            return View($"{ViewPath}Details.cshtml", employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View($"{ViewPath}Create.cshtml");
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateModel employeeCreateModel)
        {
            if (ModelState.IsValid)
            {
                var employee = _mapper.Map<Employee>(employeeCreateModel);
                await _employeeRepository.Create(employee);

                return RedirectToAction(nameof(Index));
            }

            return View($"{ViewPath}Create.cshtml", employeeCreateModel);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeEditModel = _mapper.Map<EmployeeEditModel>(await _employeeRepository.Get(id));

            if (employeeEditModel == null)
            {
                return NotFound();
            }

            return View($"{ViewPath}Edit.cshtml", employeeEditModel);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditModel employeeEditModel)
        {
            if (id != employeeEditModel.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var employee = _mapper.Map<Employee>(employeeEditModel);
                    await _employeeRepository.Update(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmployeeExists(employeeEditModel.EmployeeId))
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

            return View($"{ViewPath}Edit.cshtml", employeeEditModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int[] ids)
        {
            var employees = new List<Employee>();

            foreach (var id in ids)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _employeeRepository.Get(id);

                if (employee == null)
                {
                    return NotFound();
                }

                employees.Add(employee);
            }

            return View($"{ViewPath}Delete.cshtml", _mapper.Map<IEnumerable<EmployeeIndexModel>>(employees));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _employeeRepository.DeleteSeveral(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmployeeExists(int id)
        {
            return (await _employeeRepository.Get(id)) != null;
        }
    }
}
