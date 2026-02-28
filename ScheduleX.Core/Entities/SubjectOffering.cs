using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public class SubjectOffering
{
    [Key]
    public int OfferingId { get; set; }

    [Required]
    public int SemesterId { get; set; }

    [ForeignKey(nameof(SemesterId))]
    public Semester Semester { get; set; } = null!;

    [Required]
    public int SubjectId { get; set; }

    [ForeignKey(nameof(SubjectId))]
    public Subject Subject { get; set; } = null!;

    [Required]
    public int FacultyId { get; set; }

    [ForeignKey(nameof(FacultyId))]
    public Faculty Faculty { get; set; } = null!;

    [Required]
    public byte TheoryLecturesPerWeek { get; set; } = 0;

    [Required]
    public byte PracticalLecturesPerWeek { get; set; } = 0;

    public byte? PracticalBlockSize { get; set; } // 2 continuous slots

    public byte? PreferredRoomType { get; set; } // 1=Classroom,2=Lab (optional)

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<TimeTableEntry> TimeTableEntries { get; set; } = new List<TimeTableEntry>();
}