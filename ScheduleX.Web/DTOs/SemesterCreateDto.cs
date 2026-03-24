using System.ComponentModel.DataAnnotations;

namespace ScheduleX.Web.DTOs;

public class SemesterCreateDto
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    [Range(1, 8, ErrorMessage = "Semester must be between 1 to 8")]
    public byte SemesterNo { get; set; }
}