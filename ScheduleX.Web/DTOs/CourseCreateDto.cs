//namespace ScheduleX.Web.DTOs
//{
//    public class CourseCreateDto
//    {
//    }
//}

namespace ScheduleX.Web.DTOs;

public class CourseCreateDto
{
    public int DepartmentId { get; set; }
    public string CourseName { get; set; } = "";
    public string? CourseCode { get; set; }
    public int MaxSem { get; set; }
}