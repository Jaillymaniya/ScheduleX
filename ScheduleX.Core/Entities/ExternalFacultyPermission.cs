using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public class ExternalFacultyPermission
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FacultyId { get; set; }

    [ForeignKey(nameof(FacultyId))]
    public Faculty Faculty { get; set; } = null!;

    [Required]
    public int DepartmentId { get; set; } // Allowed department

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}