using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities;
public enum SubjectCategoryEnum : byte
{
    Theory = 1,
    Practical = 2,
    Both = 3
}

public class Subject
{
    [Key]
    public int SubjectId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select course")]
    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    [Required(ErrorMessage = "Subject name is required")]
    [MaxLength(150)]
    public string SubjectName { get; set; } = null!;
    
    [MaxLength(30)]
    public string? SubjectCode { get; set; }

    [Required(ErrorMessage = "Credits are required")]
    [Range(1, 10, ErrorMessage = "Credits must be between 1-10")]
    public int Credits { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public SubjectCategoryEnum SubjectCategory { get; set; }

    public bool IsElective { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav
    public ICollection<SubjectOffering> SubjectOfferings { get; set; } = new List<SubjectOffering>();
}