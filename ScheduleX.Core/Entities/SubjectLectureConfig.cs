using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public class SubjectLectureConfig
{
    [Key]
    public int SubjectLectureConfigId { get; set; }

    [Required]
    public int SubjectSemesterId { get; set; }

    [ForeignKey(nameof(SubjectSemesterId))]
    public SubjectSemester SubjectSemester { get; set; } = null!;

    [Required]
    [Range(0, 10)]
    public byte TheoryLecturesPerWeek { get; set; }

    [Required]
    [Range(0, 10)]
    public byte PracticalLecturesPerWeek { get; set; }

    [Range(1, 5)]
    public byte? PracticalBlockSize { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}