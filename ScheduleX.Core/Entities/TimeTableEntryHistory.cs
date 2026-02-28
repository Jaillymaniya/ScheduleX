using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;
public class TimeTableEntryHistory
{
    [Key]
    public int HistoryId { get; set; }

    [Required]
    public int EntryId { get; set; }

    [ForeignKey(nameof(EntryId))]
    public TimeTableEntry TimeTableEntry { get; set; } = null!;

    [Required]
    public int BatchId { get; set; }

    [ForeignKey(nameof(BatchId))]
    public TimeTableBatch TimeTableBatch { get; set; } = null!;

    [Required, MaxLength(20)]
    public string Action { get; set; } = null!; // INSERT/UPDATE/DELETE

    public string? OldDataJson { get; set; }

    public string? NewDataJson { get; set; }

    [Required]
    public int ChangedByUserId { get; set; }

    [ForeignKey(nameof(ChangedByUserId))]
    public User ChangedByUser { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.Now;
}