namespace ScheduleX.Web.DTOs;

public class ScheduleConfigDto
{
    public int ConfigId { get; set; }
    public int DepartmentId { get; set; }
    public int? CourseId { get; set; }
    public string? CourseName { get; set; }

    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";

    public int LectureDurationMin { get; set; }
    public int WorkingDaysMask { get; set; }
    public byte LecturesPerDay { get; set; }
    public bool IsActive { get; set; } = true;

    public List<BreakRuleDto> BreakRules { get; set; } = new();
}