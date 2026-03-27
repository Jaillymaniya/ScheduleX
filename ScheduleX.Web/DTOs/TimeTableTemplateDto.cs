namespace ScheduleX.Web.DTOs;

public class TimeTableTemplateDto
{
    public int TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public byte LayoutType { get; set; }
    public string LayoutTypeName { get; set; } = string.Empty;
    public string? TemplateJson { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UsedInBatchCount { get; set; }
}