//namespace ScheduleX.Web.DTOs
//{
//    public class CourseDto
//    {
//    }
//}


namespace ScheduleX.Web.DTOs;

public class CourseDto
{
    public int CourseId { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = "";
    public string CourseName { get; set; } = "";
    public string? CourseCode { get; set; }
    public bool IsActive { get; set; }
}