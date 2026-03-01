
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Core.Entities;
using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Controllers.Admin;

[ApiController]
[Route("api/admin/ttcoordinator")]
public class TTCoordinatorController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public TTCoordinatorController(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Users
            .Where(x => x.Role == UserRole.TTCoordinator)
            .Include(x => x.Department)
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments()
    {
        return Ok(await _context.Departments
            .Where(x => x.IsActive)
            .ToListAsync());
    }

    [HttpGet("courses/{departmentId}")]
    public async Task<IActionResult> GetCourses(int departmentId)
    {
        return Ok(await _context.Courses
            .Where(x => x.DepartmentId == departmentId && x.IsActive)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterTTCoordinatorDto dto)
    {
        if (!dto.DepartmentId.HasValue || !dto.CourseId.HasValue)
            return BadRequest("Department and Course required");

        var plainPassword = dto.Password;

        var user = new User
        {
            FullName = dto.FullName,
            Username = dto.Username,
            Email = dto.Email,
            Phone = dto.Phone,
            DepartmentId = dto.DepartmentId.Value,
            Role = UserRole.TTCoordinator,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);

        var mapping = new TTCoordinatorCourse
        {
            User = user,
            CourseId = dto.CourseId.Value
        };

        _context.TTCoordinatorCourses.Add(mapping);

        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.Email!,
            "TT Coordinator Registration",
            $"Registered Successfully\n\nUsername: {user.Username}\nPassword: {plainPassword}"
        );

        return Ok();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
        }

        return Ok();
    }
}