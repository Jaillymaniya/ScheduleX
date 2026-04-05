using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public class SubjectFaculty
{
    [Key]
    public int SubjectFacultyId { get; set; }

    [Required]
    public int SubjectSemesterId { get; set; }

    [ForeignKey(nameof(SubjectSemesterId))]
    public SubjectSemester SubjectSemester { get; set; } = null!;

    [Required]
    public int DivisionId { get; set; }

    [ForeignKey(nameof(DivisionId))]
    public Division Division { get; set; } = null!;

    [Required]
    public int FacultyId { get; set; }

    [ForeignKey(nameof(FacultyId))]
    public Faculty Faculty { get; set; } = null!;
    [Required]
    public SubjectCategoryEnum TeachingType { get; set; }
    // 1 = Theory, 2 = Practical
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}