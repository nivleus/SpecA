using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecA.Data;
using SpecA.Models;

namespace SpecA.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly AppDbContext _context;

        public CompaniesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Companies
        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies
                .Include(c => c.Departments)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(companies);
        }

        // GET: /Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var company = await _context.Companies
                .Include(c => c.Departments.OrderBy(d => d.Name))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            return View(company);
        }

        // GET: /Companies/Create
        public IActionResult Create()
        {
            return View(new Company());
        }

        // POST: /Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,Industry,Address,ContactEmail,ContactPhone,Website,IsActive")] Company company)
        {
            if (!ModelState.IsValid) return View(company);

            company.CreatedAt = DateTime.UtcNow;
            _context.Add(company);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Company \"{company.Name}\" was created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var company = await _context.Companies.FindAsync(id);
            if (company == null) return NotFound();

            return View(company);
        }

        // POST: /Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,Industry,Address,ContactEmail,ContactPhone,Website,IsActive,CreatedAt")] Company company)
        {
            if (id != company.Id) return NotFound();

            if (!ModelState.IsValid) return View(company);

            try
            {
                company.UpdatedAt = DateTime.UtcNow;
                _context.Update(company);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Company \"{company.Name}\" was updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Companies.AnyAsync(c => c.Id == company.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Details), new { id = company.Id });
        }

        // GET: /Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var company = await _context.Companies
                .Include(c => c.Departments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            return View(company);
        }

        // POST: /Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                // Departments are removed via the cascade delete configured on the relationship.
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Company \"{company.Name}\" and its departments were deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
