using System.ComponentModel.DataAnnotations;

namespace ScheduleX.Web.DTOs;

public class UpdateTimeTableTemplateDto
{
    [Required]
    public int TemplateId { get; set; }

    [Required]
    [StringLength(100)]
    public string TemplateName { get; set; } = string.Empty;

    [Required]
    public byte LayoutType { get; set; }

    public string? TemplateJson { get; set; }

    public bool IsActive { get; set; }
}