using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public enum SlotTypeEnum : byte
{
    Lecture = 1,
    Break = 2
}

public class TimeSlot
{
    [Key]
    public int TimeSlotId { get; set; }

    [Required]
    public int ConfigId { get; set; }

    [ForeignKey(nameof(ConfigId))]
    public ScheduleConfig ScheduleConfig { get; set; } = null!;

    [Required]
    public byte SlotNo { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Required]
    public SlotTypeEnum SlotType { get; set; }

    public int? BreakRuleId { get; set; }

    [ForeignKey(nameof(BreakRuleId))]
    public BreakRule? BreakRule { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<TimeTableEntry> TimeTableEntries { get; set; } = new List<TimeTableEntry>();
}