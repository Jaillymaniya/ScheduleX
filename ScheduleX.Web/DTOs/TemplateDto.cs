using ScheduleX.Core.Entities;

namespace ScheduleX.Web.DTOs;

public class TemplateDto
{
    public int TemplateId { get; set; }
    public string TemplateName { get; set; } = "";
    public LayoutTypeEnum LayoutType { get; set; }
    public bool IsDefault { get; set; }
}