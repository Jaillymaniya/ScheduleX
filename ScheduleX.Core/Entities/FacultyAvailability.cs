using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;

public class FacultyAvailability
{
    [Key]
    public int AvailabilityId { get; set; }

    [Required]
    public int FacultyId { get; set; }

    [ForeignKey(nameof(FacultyId))]
    public Faculty Faculty { get; set; } = null!;

    [Required]
    public byte DayOfWeek { get; set; } // 1..7

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}