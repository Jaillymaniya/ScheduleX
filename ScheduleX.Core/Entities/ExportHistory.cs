using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;
public enum ExportTypeEnum : byte
{
    Pdf = 1,
    Excel = 2
}

public class ExportHistory
{
    [Key]
    public int ExportId { get; set; }

    [Required]
    public int BatchId { get; set; }

    [ForeignKey(nameof(BatchId))]
    public TimeTableBatch TimeTableBatch { get; set; } = null!;

    [Required]
    public ExportTypeEnum ExportType { get; set; }

    [Required]
    public int TemplateSnapshotId { get; set; }

    [ForeignKey(nameof(TemplateSnapshotId))]
    public BatchTemplateSnapshot BatchTemplateSnapshot { get; set; } = null!;

    [Required]
    public int ExportedByUserId { get; set; }

    [ForeignKey(nameof(ExportedByUserId))]
    public User ExportedByUser { get; set; } = null!;

    public DateTime ExportedAt { get; set; } = DateTime.Now;
}