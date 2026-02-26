using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timetable.Core.Entities;

public enum UserRole : byte
{
    Admin = 1,
    TTCoordinator = 2
}

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public UserRole Role { get; set; }

    public int? DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department? Department { get; set; }

    [MaxLength(120)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    //public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    public ICollection<TimeTableBatch> CreatedBatches { get; set; } = new List<TimeTableBatch>();
    public ICollection<TimeTableEntryHistory> TimeTableEntryHistories { get; set; } = new List<TimeTableEntryHistory>();
    public ICollection<ExportHistory> ExportHistories { get; set; } = new List<ExportHistory>();
}