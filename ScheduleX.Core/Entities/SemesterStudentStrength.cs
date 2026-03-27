using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public class SemesterStudentStrength
{
    [Key]
    public int Id { get; set; }

    // =========================
    // Semester Reference
    // =========================
    [Required]
    public int SemesterId { get; set; }

    [ForeignKey(nameof(SemesterId))]
    public Semester Semester { get; set; } = null!;

    // =========================
    // Total Students
    // =========================
    [Required]
    [Range(1, 1000)] // optional validation
    public int TotalStudents { get; set; }

    // =========================
    // Metadata
    // =========================
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}