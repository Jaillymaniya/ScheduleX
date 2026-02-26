using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timetable.Core.Entities;

public enum RoomTypeEnum : byte
{
    Classroom = 1,
    Lab = 2
}

public class Room
{
    [Key]
    public int RoomId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = null!;

    [Required, MaxLength(50)]
    public string RoomName { get; set; } = null!; // UNIQUE

    [Required]
    public RoomTypeEnum RoomType { get; set; }

    [Required]
    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<DivisionRoomAllocation> DivisionRoomAllocations { get; set; } = new List<DivisionRoomAllocation>();
    public ICollection<TimeTableEntry> TimeTableEntries { get; set; } = new List<TimeTableEntry>();
}