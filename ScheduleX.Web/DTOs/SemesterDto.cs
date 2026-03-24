namespace ScheduleX.Web.DTOs;

public class SemesterDto
{
    public int SemesterId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public byte SemesterNo { get; set; }
    public bool IsActive { get; set; }
}