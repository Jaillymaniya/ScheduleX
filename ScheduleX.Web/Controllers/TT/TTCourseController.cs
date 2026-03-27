using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Web.Controllers.TT;

[ApiController]
[Route("api/tt/course")]
public class TTCourseController : ControllerBase
{
    private readonly AppDbContext _context;

    public TTCourseController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("my-course/{userId}")]
    public async Task<IActionResult> GetMyCourse(int userId)
    {
        var course = await _context.TTCoordinatorCourses
            .Where(x => x.UserId == userId)
            .Include(x => x.Course)
            .Select(x => new CourseDto
            {
                CourseId = x.Course.CourseId,
                CourseName = x.Course.CourseName,
                MaxSem = x.Course.MaxSem,
                DepartmentId = x.Course.DepartmentId
            })
            .FirstOrDefaultAsync();

        if (course == null)
            return NotFound("No course allocated to this TT");

        return Ok(course);
    }
}

public class CourseDto
{
    public int CourseId { get; set; }

    public int DepartmentId { get; set; }
    public string CourseName { get; set; } = "";
    public int MaxSem { get; set; }
}