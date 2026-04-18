namespace ScheduleX.Web.DTOs
{
    public class GenerateTTDto
    {
        public int CourseId { get; set; }
        public List<int> SemesterIds { get; set; } = new();
        public int TemplateId { get; set; }
        public int UserId { get; set; }
    }
}
