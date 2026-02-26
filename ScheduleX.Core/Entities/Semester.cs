using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timetable.Core.Entities;

public class Semester
{
    [Key]
    public int SemesterId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    [Required]
    public byte SemesterNo { get; set; } // 1..8

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<Division> Divisions { get; set; } = new List<Division>();
    public ICollection<DivisionRoomAllocation> DivisionRoomAllocations { get; set; } = new List<DivisionRoomAllocation>();
    public ICollection<SubjectOffering> SubjectOfferings { get; set; } = new List<SubjectOffering>();
    public ICollection<TimeTableBatchSemester> BatchSemesters { get; set; } = new List<TimeTableBatchSemester>();
    public ICollection<TimeTableEntry> TimeTableEntries { get; set; } = new List<TimeTableEntry>();
}