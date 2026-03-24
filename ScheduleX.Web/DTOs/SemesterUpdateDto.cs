using System.ComponentModel.DataAnnotations;

namespace ScheduleX.Web.DTOs;

public class SemesterUpdateDto
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    [Range(1, 8)]
    public byte SemesterNo { get; set; }

    public bool IsActive { get; set; }
}