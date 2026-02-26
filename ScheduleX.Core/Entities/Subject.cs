using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timetable.Core.Entities;

public enum SubjectCategoryEnum : byte
{
    Theory = 1,
    Practical = 2,
    Both = 3
}

public class Subject
{
    [Key]
    public int SubjectId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = null!;

    [Required, MaxLength(150)]
    public string SubjectName { get; set; } = null!;

    [MaxLength(30)]
    public string? SubjectCode { get; set; }

    [Required]
    public byte Credits { get; set; }

    [Required]
    public SubjectCategoryEnum SubjectCategory { get; set; }

    public bool IsElective { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<SubjectOffering> SubjectOfferings { get; set; } = new List<SubjectOffering>();
}