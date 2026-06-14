using System.ComponentModel.DataAnnotations;

namespace SpecA.Models.ViewModels
{
    /// <summary>
    /// Backs the department create/edit forms, carrying the owning company context
    /// (id + display name) alongside the editable department fields.
    /// </summary>
    public class DepartmentFormViewModel
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

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
    }
}
