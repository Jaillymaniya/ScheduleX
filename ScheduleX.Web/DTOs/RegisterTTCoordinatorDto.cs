//using System.ComponentModel.DataAnnotations;

//namespace ScheduleX.Web.DTOs;

//public class RegisterTTCoordinatorDto
//{
//    [Required]
//    public string FullName { get; set; } = "";

//    [Required]
//    public string Username { get; set; } = "";

//    [Required, EmailAddress]
//    public string Email { get; set; } = "";

//    public string? Phone { get; set; }

//    [Required, MinLength(6)]
//    public string Password { get; set; } = "";

//    [Required]
//    public int? DepartmentId { get; set; }

//    [Required]
//    public int? CourseId { get; set; } = 1;
//}

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ScheduleX.Web.DTOs;

public class RegisterTTCoordinatorDto
{
    [Required]
    [RegularExpression(@"^[A-Za-z\s]+$",
        ErrorMessage = "Full Name must contain only letters and spaces.")]
    public string FullName { get; set; } = "";

    [Required]
    public string Username { get; set; } = "";

    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string Email { get; set; } = "";

    [Required]
    [RegularExpression(@"^(?!0000000000)(?!9999999999)[1-9][0-9]{9}$",
        ErrorMessage = "Phone must be 10 digits and cannot be all 0s or all 9s.")]
    public string? Phone { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&]).{6,}$",
        ErrorMessage = "Password must contain at least one letter, one number, and one special character.")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Department is required.")]
    public int? DepartmentId { get; set; }

    [Required(ErrorMessage = "Course is required.")]
    public int? CourseId { get; set; }
}