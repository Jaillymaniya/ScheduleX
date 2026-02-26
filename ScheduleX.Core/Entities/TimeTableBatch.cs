using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timetable.Core.Entities;

public enum BatchStatusEnum : byte
{
    Draft = 1,
    Generated = 2,
    Published = 3,
    Archived = 4
}

public class TimeTableBatch
{
    [Key]
    public int BatchId { get; set; }

    [Required]
    public int CreatedByUserId { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public User CreatedByUser { get; set; } = null!;

    [Required]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = null!;

    [Required]
    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    [Required]
    public int ConfigId { get; set; }

    [ForeignKey(nameof(ConfigId))]
    public ScheduleConfig ScheduleConfig { get; set; } = null!;

    [Required]
    public int TemplateId { get; set; }

    [ForeignKey(nameof(TemplateId))]
    public TimeTableTemplate TimeTableTemplate { get; set; } = null!;

    [Required]
    public int VersionNo { get; set; }

    public int? ParentBatchId { get; set; }

    [ForeignKey(nameof(ParentBatchId))]
    public TimeTableBatch? ParentBatch { get; set; }

    [Required]
    public BatchStatusEnum Status { get; set; }

    public bool IsActiveVersion { get; set; } = false;

    public DateTime? GeneratedAt { get; set; }

    [MaxLength(250)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<TimeTableBatchSemester> BatchSemesters { get; set; } = new List<TimeTableBatchSemester>();
    public ICollection<TimeTableEntry> TimeTableEntries { get; set; } = new List<TimeTableEntry>();
    public BatchTemplateSnapshot? BatchTemplateSnapshot { get; set; }
    public ICollection<ExportHistory> ExportHistories { get; set; } = new List<ExportHistory>();
}