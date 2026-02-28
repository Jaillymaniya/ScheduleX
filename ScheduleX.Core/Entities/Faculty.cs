using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public class Faculty
{
    [Key]
    public int FacultyId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = null!;

    [Required, MaxLength(120)]
    public string FacultyName { get; set; } = null!;

    [MaxLength(30)]
    public string? FacultyCode { get; set; }

    [MaxLength(120)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public byte? MaxLecturesPerDay { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<FacultyAvailability> FacultyAvailabilities { get; set; } = new List<FacultyAvailability>();
    public ICollection<SubjectOffering> SubjectOfferings { get; set; } = new List<SubjectOffering>();
}