using SpecA.Models;

namespace SpecA.Models.ViewModels
{
    /// <summary>
    /// Summary data for the dashboard: total counts and the most recent records.
    /// </summary>
    public class DashboardViewModel
    {
        public int CompanyCount { get; set; }
        public int DepartmentCount { get; set; }
        public List<Company> RecentCompanies { get; set; } = new();
        public List<Department> RecentDepartments { get; set; } = new();
    }
}
