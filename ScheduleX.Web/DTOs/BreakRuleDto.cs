namespace ScheduleX.Web.DTOs;

public class BreakRuleDto
{
    public int BreakRuleId { get; set; }
    public int ConfigId { get; set; }
    public byte BreakNo { get; set; }
    public byte AfterLectureNo { get; set; }
    public int BreakDurationMin { get; set; }
    public string? BreakName { get; set; }
}