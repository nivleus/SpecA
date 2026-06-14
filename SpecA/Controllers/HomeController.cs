using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecA.Data;
using SpecA.Models;
using SpecA.Models.ViewModels;
using System.Diagnostics;

namespace SpecA.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                CompanyCount = await _context.Companies.CountAsync(),
                DepartmentCount = await _context.Departments.CountAsync(),
                RecentCompanies = await _context.Companies
                    .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                RecentDepartments = await _context.Departments
                    .Include(d => d.Company)
                    .OrderByDescending(d => d.UpdatedAt ?? d.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
