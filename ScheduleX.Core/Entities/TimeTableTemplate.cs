using System.ComponentModel.DataAnnotations;

namespace Timetable.Core.Entities;

public enum LayoutTypeEnum : byte
{
    Grid = 1,
    Compact = 2,
    Detailed = 3
}

public class TimeTableTemplate
{
    [Key]
    public int TemplateId { get; set; }

    [Required, MaxLength(100)]
    public string TemplateName { get; set; } = null!;

    [Required]
    public LayoutTypeEnum LayoutType { get; set; }

    public string? TemplateJson { get; set; } // NVARCHAR(MAX)

    public bool IsDefault { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<TimeTableBatch> TimeTableBatches { get; set; } = new List<TimeTableBatch>();
    public ICollection<BatchTemplateSnapshot> TemplateSnapshots { get; set; } = new List<BatchTemplateSnapshot>();
}