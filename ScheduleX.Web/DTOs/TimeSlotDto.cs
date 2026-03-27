using ScheduleX.Core.Entities;
namespace ScheduleX.Web.DTOs;

public class TimeSlotDto
{
    public int TimeSlotId { get; set; }
    public int ConfigId { get; set; }
    public byte SlotNo { get; set; }
    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";
    public SlotTypeEnum SlotType { get; set; }
    public int? BreakRuleId { get; set; }
    public string? BreakName { get; set; }
}