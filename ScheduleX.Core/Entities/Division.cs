using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public class Division
{
    [Key]
    public int DivisionId { get; set; }

    [Required]
    public int SemesterId { get; set; }

    [ForeignKey(nameof(SemesterId))]
    public Semester Semester { get; set; } = null!;

    [Required, MaxLength(20)]
    public string DivisionName { get; set; } = null!; // A/B/C

    [Required]
    public int StudentStrength { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<DivisionRoomAllocation> DivisionRoomAllocations { get; set; } = new List<DivisionRoomAllocation>();
    public ICollection<TimeTableEntry> TimeTableEntries { get; set; } = new List<TimeTableEntry>();
}