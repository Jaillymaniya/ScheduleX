//namespace ScheduleX.Web.DTOs
//{
//    public class CourseUpdateDto
//    {
//    }
//}

namespace ScheduleX.Web.DTOs;

public class CourseUpdateDto
{
    public int DepartmentId { get; set; }
    public string CourseName { get; set; } = "";
    public string? CourseCode { get; set; }
    public bool IsActive { get; set; }
}