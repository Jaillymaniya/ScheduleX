using System.ComponentModel.DataAnnotations;

namespace Timetable.Core.Entities;

public class Department
{
    [Key]
    public int DepartmentId { get; set; }

    [Required, MaxLength(100)]
    public string DepartmentName { get; set; } = null!;

    [MaxLength(20)]
    public string? DepartmentCode { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<ScheduleConfig> ScheduleConfigs { get; set; } = new List<ScheduleConfig>();
    public ICollection<TimeTableBatch> TimeTableBatches { get; set; } = new List<TimeTableBatch>();
}