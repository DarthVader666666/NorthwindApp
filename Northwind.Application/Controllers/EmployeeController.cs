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
    public class EmployeeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeeController(IRepository<Employee> employeeRepository, IMapper mapper)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetListAsync();
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

            var employeeDetailsModel = _mapper.Map<EmployeeDetailsModel>(employee);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Employee");

            return View(employeeDetailsModel);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.ReportsTo = GetReportsToSelectList();
            ViewBag.PreviousPage = Url.ActionLink("Index", "Employee");

            var employeeCreateModel = new EmployeeCreateModel();
            employeeCreateModel.ReportsToList = GetReportsToSelectList();

            return View(employeeCreateModel);
        }

        [Authorize(Roles = "admin")]
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

            employeeCreateModel.ReportsToList = GetReportsToSelectList();

            return View(employeeCreateModel);
        }

        [Authorize(Roles = "admin")]
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

            employeeEditModel.ReportsToList = GetReportsToSelectList(id: id, reportsTo: employeeEditModel.ReportsTo ?? 0);

            return View(employeeEditModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditModel employeeEditModel)
        {
            if (id != employeeEditModel.EmployeeId)
            {
                return RedirectToAction(nameof(Index));
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

            employeeEditModel.ReportsToList = GetReportsToSelectList(id: id, reportsTo:employeeEditModel.ReportsTo);
            return View(employeeEditModel);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] int[] ids)
        {
            var employees = await _employeeRepository.GetRangeAsync(ids);

            ViewBag.PreviousPage = Url.ActionLink("Index", "Employee");

            return View(_mapper.Map<IEnumerable<EmployeeIndexModel>>(employees));
        }

        [Authorize(Roles = "admin")]
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

        private SelectList GetReportsToSelectList(int? id = null, int? reportsTo = null)
        {
            var list = _employeeRepository.GetListAsync().Result;
            var dictionary = list.Except(list.Where(e => e.EmployeeId == id)).ToDictionary(e => e.EmployeeId, e => e.FirstName + " " + e.LastName);

            dictionary.Add(0, "");
            var selectList = new SelectList(dictionary, "Key", "Value");

            SelectListItem selectedItem = null;

            if (reportsTo != null)
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == reportsTo.ToString());                
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
