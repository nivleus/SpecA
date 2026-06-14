using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecA.Data;
using SpecA.Models;
using SpecA.Models.ViewModels;

namespace SpecA.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Departments/Create?companyId=5
        public async Task<IActionResult> Create(int? companyId)
        {
            if (companyId == null) return NotFound();

            var company = await _context.Companies.FindAsync(companyId);
            if (company == null) return NotFound();

            var vm = new DepartmentFormViewModel
            {
                CompanyId = company.Id,
                CompanyName = company.Name
            };
            return View(vm);
        }

        // POST: /Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentFormViewModel form)
        {
            var company = await _context.Companies.FindAsync(form.CompanyId);
            if (company == null) return NotFound();
            form.CompanyName = company.Name;

            await ValidateUniqueNameAsync(form);

            if (!ModelState.IsValid) return View(form);

            var department = new Department
            {
                CompanyId = form.CompanyId,
                Name = form.Name.Trim(),
                Code = form.Code,
                Description = form.Description,
                Manager = form.Manager,
                Phone = form.Phone,
                IsActive = form.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Department \"{department.Name}\" was added.";
            return RedirectToAction("Details", "Companies", new { id = form.CompanyId });
        }

        // GET: /Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .Include(d => d.Company)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return NotFound();

            var vm = new DepartmentFormViewModel
            {
                Id = department.Id,
                CompanyId = department.CompanyId,
                CompanyName = department.Company?.Name ?? string.Empty,
                Name = department.Name,
                Code = department.Code,
                Description = department.Description,
                Manager = department.Manager,
                Phone = department.Phone,
                IsActive = department.IsActive
            };
            return View(vm);
        }

        // POST: /Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentFormViewModel form)
        {
            if (id != form.Id) return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            // Company is fixed for an existing department.
            form.CompanyId = department.CompanyId;
            form.CompanyName = (await _context.Companies.FindAsync(department.CompanyId))?.Name ?? string.Empty;

            await ValidateUniqueNameAsync(form);

            if (!ModelState.IsValid) return View(form);

            department.Name = form.Name.Trim();
            department.Code = form.Code;
            department.Description = form.Description;
            department.Manager = form.Manager;
            department.Phone = form.Phone;
            department.IsActive = form.IsActive;
            department.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Department \"{department.Name}\" was updated.";
            return RedirectToAction("Details", "Companies", new { id = department.CompanyId });
        }

        // GET: /Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .Include(d => d.Company)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return NotFound();

            return View(department);
        }

        // POST: /Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            var companyId = department.CompanyId;
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Department \"{department.Name}\" was deleted.";
            return RedirectToAction("Details", "Companies", new { id = companyId });
        }

        // Enforces department-name uniqueness within the parent company (case-insensitive).
        private async Task ValidateUniqueNameAsync(DepartmentFormViewModel form)
        {
            if (string.IsNullOrWhiteSpace(form.Name)) return;

            var name = form.Name.Trim();
            var duplicate = await _context.Departments
                .AnyAsync(d => d.CompanyId == form.CompanyId
                            && d.Id != form.Id
                            && d.Name.ToLower() == name.ToLower());

            if (duplicate)
            {
                ModelState.AddModelError(nameof(form.Name),
                    $"A department named \"{name}\" already exists for this company.");
            }
        }
    }
}
