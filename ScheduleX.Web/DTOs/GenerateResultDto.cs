using ScheduleX.Web.Models.Template;
namespace ScheduleX.Web.DTOs
{
    public class GenerateResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Base64 { get; set; }
        public List<PreviewDto>? Preview { get; set; }
        public TemplateStyle? TemplateStyle { get; set; }
    }
}
