using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timetable.Core.Entities;

public class BatchTemplateSnapshot
{
    [Key]
    public int SnapshotId { get; set; }

    [Required]
    public int BatchId { get; set; }

    [ForeignKey(nameof(BatchId))]
    public TimeTableBatch TimeTableBatch { get; set; } = null!;

    [Required]
    public int TemplateId { get; set; }

    [ForeignKey(nameof(TemplateId))]
    public TimeTableTemplate TimeTableTemplate { get; set; } = null!;

    [Required]
    public string TemplateJsonSnapshot { get; set; } = null!; // NVARCHAR(MAX)

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}