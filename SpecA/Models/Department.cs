using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpecA.Models
{
    /// <summary>
    /// An organizational unit belonging to a single company.
    /// Department name is unique within its parent company.
    /// </summary>
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Department Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(150)]
        public string? Manager { get; set; }

        [StringLength(50)]
        [Phone]
        public string? Phone { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}
