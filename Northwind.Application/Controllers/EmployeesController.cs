﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Employee;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = "admin")]
    public class EmployeesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository, IMapper mapper)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            var employees = _employeeRepository.GetList();
            var employeeModels = _mapper.Map<IEnumerable<EmployeeIndexModel>>(employees);

            return View(employeeModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.ReportsToNavigation = await _employeeRepository.GetAsync(employee.ReportsTo);

            return View(employee);
        }

        public IActionResult Create()
        {
            ViewBag.ReportsTo = GetReportsToSelectList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateModel employeeCreateModel)
        {
            if (ModelState.IsValid)
            {
                var employee = _mapper.Map<Employee>(employeeCreateModel);
                await _employeeRepository.CreateAsync(employee);

                return RedirectToAction(nameof(Index));
            }

            return View(employeeCreateModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeEditModel = _mapper.Map<EmployeeEditModel>(await _employeeRepository.GetAsync(id));

            if (employeeEditModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ReportsTo = GetReportsToSelectList(id);

            return View(employeeEditModel);
        }

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
                    await _employeeRepository.UpdateAsync(employee);
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

            return View(employeeEditModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var employees = new List<Employee>();

            foreach (var id in ids)
            {
                var employee = await _employeeRepository.GetAsync(id);

                if (employee == null)
                {
                    return NotFound();
                }

                employees.Add(employee);
            }

            return View(_mapper.Map<IEnumerable<EmployeeIndexModel>>(employees));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int[] ids)
        {
            await _employeeRepository.DeleteSeveralAsync(ids);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmployeeExists(int id)
        {
            return (await _employeeRepository.GetAsync(id)) != null;
        }

        private SelectList GetReportsToSelectList(int? id = null)
        {
            var list = _employeeRepository.GetList();
            var dictionary = list.Except(list.Where(e => e.EmployeeId == id)).ToDictionary(e => e.EmployeeId, e => e.FirstName + " " + e.LastName);

            dictionary.Add(0, "");
            return new SelectList(dictionary, "Key", "Value");
        }
    }
}
