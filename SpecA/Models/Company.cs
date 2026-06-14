using System.ComponentModel.DataAnnotations;

namespace SpecA.Models
{
    /// <summary>
    /// An organization tracked by the application. A company has zero or more departments.
    /// </summary>
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Company Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(150)]
        [EmailAddress]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        [Phone]
        [Display(Name = "Contact Phone")]
        public string? ContactPhone { get; set; }

        [StringLength(200)]
        [Url]
        public string? Website { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
