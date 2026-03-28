//using System.ComponentModel.DataAnnotations;

//namespace ScheduleX.Web.DTOs
//{
//    public class SaveLectureConfigRequestDto
//    {
//    } 
//}
using System.ComponentModel.DataAnnotations;

namespace ScheduleX.Web.DTOs;

public class SaveLectureConfigRequestDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int SemesterId { get; set; }

    [Required]
    public List<LectureConfigRowDto> Rows { get; set; } = new();
}