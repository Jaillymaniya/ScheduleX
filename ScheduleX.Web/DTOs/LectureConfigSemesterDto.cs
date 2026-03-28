//namespace ScheduleX.Web.DTOs
//{
//    public class LectureConfigSemesterDto
//    {
//    }
//}
namespace ScheduleX.Web.DTOs;

public class LectureConfigSemesterDto
{
    public int SemesterId { get; set; }
    public int SemesterNo { get; set; }
    public string DisplayName => $"Semester {SemesterNo}";
}