//namespace ScheduleX.Web.Controllers.TT
//{
//    public class SemesterController
//    {
//    }
//}
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Web.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ScheduleX.Web.Controllers.TT;

[ApiController]
[Route("api/tt/semester")]
public class SemesterController : ControllerBase
{
    private readonly ISemesterRepository _repo;
    private readonly AppDbContext _context;

    public SemesterController(ISemesterRepository repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repo.GetAllAsync();

        var result = data.Select(s => new SemesterDto
        {
            SemesterId = s.SemesterId,
            CourseId = s.CourseId,
            CourseName = s.Course?.CourseName ?? "",
            SemesterNo = s.SemesterNo,
            IsActive = s.IsActive
        }).ToList();

        return Ok(result);
    }

    [HttpGet("by-course/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var data = await _repo.GetByCourseAsync(courseId);

        var result = data.Select(s => new SemesterDto
        {
            SemesterId = s.SemesterId,
            CourseId = s.CourseId,
            CourseName = s.Course?.CourseName ?? "",
            SemesterNo = s.SemesterNo,
            IsActive = s.IsActive
        }).ToList();

        return Ok(result);
    }

    //[HttpPost]
    //public async Task<IActionResult> Create(SemesterCreateDto dto)
    //{
    //    try
    //    {
    //        var semester = new Semester
    //        {
    //            CourseId = dto.CourseId,
    //            SemesterNo = dto.SemesterNo,
    //            IsActive = true,
    //            CreatedAt = DateTime.Now
    //        };

    //        await _repo.AddAsync(semester);
    //        return Ok();
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}
    [HttpPost]
    public async Task<IActionResult> Create(SemesterCreateDto dto)
    {
        try
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.CourseId == dto.CourseId);

            if (course == null)
                return BadRequest("Course not found");

            if (dto.SemesterNo < 1 || dto.SemesterNo > course.MaxSem)
                return BadRequest($"Semester must be between 1 and {course.MaxSem}");

            var semester = new Semester
            {
                CourseId = dto.CourseId,
                SemesterNo = dto.SemesterNo,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(semester);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(int id, SemesterUpdateDto dto)
    //{
    //    try
    //    {
    //        var semester = new Semester
    //        {
    //            SemesterId = id,
    //            CourseId = dto.CourseId,
    //            SemesterNo = dto.SemesterNo,
    //            IsActive = dto.IsActive
    //        };

    //        await _repo.UpdateAsync(semester);
    //        return Ok();
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SemesterUpdateDto dto)
    {
        try
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.CourseId == dto.CourseId);

            if (course == null)
                return BadRequest("Course not found");

            if (dto.SemesterNo < 1 || dto.SemesterNo > course.MaxSem)
                return BadRequest($"Semester must be between 1 and {course.MaxSem}");

            var semester = new Semester
            {
                SemesterId = id,
                CourseId = dto.CourseId,
                SemesterNo = dto.SemesterNo,
                IsActive = dto.IsActive
            };

            await _repo.UpdateAsync(semester);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



    [HttpPatch("{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        await _repo.ToggleStatusAsync(id);
        return Ok();
    }
}