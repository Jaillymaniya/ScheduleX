//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ScheduleX.Core.Entities;

//public enum EntryTypeEnum : byte
//{
//    Lecture = 1,
//    Break = 2,
//    Free = 3
//}

//public class TimeTableEntry
//{
//    [Key]
//    public int EntryId { get; set; }

//    public int BatchId { get; set; }
//    public int SemesterId { get; set; }
//    public int DivisionId { get; set; }

//    public byte DayOfWeek { get; set; }
//    public int TimeSlotId { get; set; }

//    public EntryTypeEnum EntryType { get; set; }

//    // 🔥 NEW (IMPORTANT)
//    public int? SubjectSemesterId { get; set; }
//    public SubjectSemester? SubjectSemester { get; set; }

//    public int? FacultyId { get; set; }
//    public Faculty? Faculty { get; set; }

//    public int? RoomId { get; set; }
//    public Room? Room { get; set; }

//    public Guid? BlockId { get; set; }
//    public byte? BlockPart { get; set; }

//    public DateTime CreatedAt { get; set; } = DateTime.Now;
//    public ICollection<TimeTableEntryHistory> Histories { get; set; } = new List<TimeTableEntryHistory>();
//}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public enum EntryTypeEnum : byte
{
    Lecture = 1,
    Break = 2,
    Free = 3
}

public class TimeTableEntry
{
    [Key]
    public int EntryId { get; set; }

    [Required]
    public int BatchId { get; set; }

    [ForeignKey(nameof(BatchId))]
    public TimeTableBatch TimeTableBatch { get; set; } = null!;

    [Required]
    public int SemesterId { get; set; }

    [ForeignKey(nameof(SemesterId))]
    public Semester Semester { get; set; } = null!;

    [Required]
    public int DivisionId { get; set; }

    [ForeignKey(nameof(DivisionId))]
    public Division Division { get; set; } = null!;

    [Required]
    public byte DayOfWeek { get; set; }

    [Required]
    public int TimeSlotId { get; set; }

    [ForeignKey(nameof(TimeSlotId))]
    public TimeSlot TimeSlot { get; set; } = null!;

    [Required]
    public EntryTypeEnum EntryType { get; set; }

    // ✅ NEW (REPLACEMENT)
    public int? SubjectSemesterId { get; set; }

    [ForeignKey(nameof(SubjectSemesterId))]
    public SubjectSemester? SubjectSemester { get; set; }

    public int? FacultyId { get; set; }

    [ForeignKey(nameof(FacultyId))]
    public Faculty? Faculty { get; set; }

    public int? RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public Room? Room { get; set; }

    public Guid? BlockId { get; set; }

    public byte? BlockPart { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<TimeTableEntryHistory> Histories { get; set; } = new List<TimeTableEntryHistory>();
}