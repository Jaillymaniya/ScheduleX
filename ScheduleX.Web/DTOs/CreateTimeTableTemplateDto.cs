using System.ComponentModel.DataAnnotations;

namespace ScheduleX.Web.DTOs;

public class CreateTimeTableTemplateDto
{
    [Required]
    [StringLength(100)]
    public string TemplateName { get; set; } = string.Empty;

    [Required]
    public byte LayoutType { get; set; }

    public string? TemplateJson { get; set; }

    public bool IsDefault { get; set; }
}