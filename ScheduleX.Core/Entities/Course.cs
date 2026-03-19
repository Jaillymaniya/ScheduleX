using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;
public class Course
{
    [Key]
    public int CourseId { get; set; }


    [Required]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; }

    [Required, MaxLength(100)]
    public string CourseName { get; set; } = null!;

    [MaxLength(20)]
    public string? CourseCode { get; set; }

    [Required]
    [Range(1, 12)]
    public int MaxSem { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<Semester> Semesters { get; set; } = new List<Semester>();
    public ICollection<TTCoordinatorCourse> TTCoordinatorCourses { get; set; }
    = new List<TTCoordinatorCourse>();
    public ICollection<ScheduleConfig> ScheduleConfigs { get; set; } = new List<ScheduleConfig>();
    public ICollection<TimeTableBatch> TimeTableBatches { get; set; } = new List<TimeTableBatch>();
}