using System.ComponentModel.DataAnnotations;

namespace ScheduleX.Web.DTOs;

public class RegisterTTCoordinatorDto
{
    [Required]
    public string FullName { get; set; } = "";

    [Required]
    public string Username { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public string? Phone { get; set; }

    [Required, MinLength(6)]
    public string Password { get; set; } = "";

    [Required]
    public int? DepartmentId { get; set; }

    [Required]
    public int? CourseId { get; set; } = 1;
}