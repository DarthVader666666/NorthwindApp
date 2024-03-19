using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Employee;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly NorthwindDbContext _context;
        private readonly IMapper _mapper;   
        private static byte[] OleHeader = new byte[] { 21, 28, 47, 0, 2, 0, 0, 0, 13, 0, 14, 0, 20,
            0, 33, 0, 255, 255, 255, 255, 66, 105, 116, 109, 97, 112, 32, 73, 109, 97, 103, 101, 0, 80, 97,
            105, 110, 116, 46, 80, 105, 99, 116, 117, 114, 101, 0, 1, 5, 0, 0, 2, 0, 0, 0, 7, 0, 0, 0, 80,
            66, 114, 117, 115, 104, 0, 0, 0, 0, 0, 0, 0, 0, 0, 32, 84, 0, 0 };

        public EmployeesController(NorthwindDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var northwindDbContext = _context.Employees.Include(e => e.ReportsToNavigation);
            await northwindDbContext.ForEachAsync(x => x.Photo = ConvertNorthwindPhoto(x.Photo!));

            return View(await northwindDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ReportsToNavigation)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeId", "FirstName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,LastName,FirstName,Title,TitleOfCourtesy,BirthDate,HireDate,Address,City,Region,PostalCode," +
            "Country,HomePhone,Extension,Photo,Notes,ReportsTo,PhotoPath,FormFile")] EmployeeEditModel employeeEditModel)
        {
            if (ModelState.IsValid)
            {
                var employee = _mapper.Map<Employee>(employeeEditModel);

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", employeeEditModel.ReportsTo);
            return View(employeeEditModel);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _mapper.Map<EmployeeEditModel>(await _context.Employees.FindAsync(id));

            if (employee == null)
            {
                return NotFound();
            }
            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", employee.ReportsTo);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employeeEditModel.EmployeeId))
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
            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", employeeEditModel.ReportsTo);
            return View(employeeEditModel);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ReportsToNavigation)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private static bool HasHeader(byte[] source, byte[] header)
        {
            if (source.Length < header.Length)
            {
                return false;
            }

            for (int i = 0; i < header.Length; i++)
            {
                if (source[i] != header[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static byte[] ConvertNorthwindPhoto(byte[] source) =>
            HasHeader(source, OleHeader) ? source[OleHeader.Length..] : source;

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
