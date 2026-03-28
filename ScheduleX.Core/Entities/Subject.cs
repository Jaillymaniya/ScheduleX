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
   
    // ✅ NEW FIELDS
    [Range(0, 10, ErrorMessage = "Theory credits must be between 0-10")]
    public int TheoryCredits { get; set; } = 0;

    [Range(0, 10, ErrorMessage = "Practical credits must be between 0-10")]
    public int PracticalCredits { get; set; } = 0;

    [Required(ErrorMessage = "Category is required")]
    public SubjectCategoryEnum SubjectCategory { get; set; }

    public bool IsElective { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    // ✅ OPTIONAL (calculated property)
    [NotMapped]
    public int TotalCredits => TheoryCredits + PracticalCredits;
    // Nav
    //public ICollection<SubjectOffering> SubjectOfferings { get; set; } = new List<SubjectOffering>();
    public ICollection<SubjectSemester> SubjectSemesters { get; set; } = new List<SubjectSemester>();
    public ICollection<SubjectFaculty> SubjectFaculties { get; set; } = new List<SubjectFaculty>();
    public ICollection<SubjectLectureConfig> SubjectLectureConfigs { get; set; } = new List<SubjectLectureConfig>();
    public ICollection<SubjectRoomConfig> SubjectRoomConfigs { get; set; } = new List<SubjectRoomConfig>();
}