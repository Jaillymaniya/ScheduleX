//namespace ScheduleX.Web.DTOs
//{
//    public class LectureConfigRowDto
//    {
//    }
//}
namespace ScheduleX.Web.DTOs;

public class LectureConfigRowDto
{
    public int SubjectLectureConfigId { get; set; }
    public int SubjectSemesterId { get; set; }
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;
    public string? SubjectCode { get; set; }

    public int TheoryCredits { get; set; }
    public int? PracticalCredits { get; set; }

    public int SubjectCategory { get; set; }
    public string SubjectCategoryName { get; set; } = string.Empty;

    public int TheoryLecturesPerWeek { get; set; }
    public int PracticalLecturesPerWeek { get; set; }
    public int? PracticalBlockSize { get; set; }
}