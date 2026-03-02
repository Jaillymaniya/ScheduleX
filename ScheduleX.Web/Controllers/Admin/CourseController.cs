//namespace ScheduleX.Web.Controllers.Admin
//{
//    public class CourseController
//    {
//    }
//}


using Microsoft.AspNetCore.Mvc;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;
using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Controllers.Admin;

[ApiController]
[Route("api/admin/course")]
public class CourseController : ControllerBase
{
    private readonly ICourseRepository _repo;

    public CourseController(ICourseRepository repo)
    {
        _repo = repo;
    }

    //[HttpGet]
    //public async Task<IActionResult> GetAll()
    //    => Ok(await _repo.GetAllAsync());

    //// department wise
    //[HttpGet("by-department/{departmentId}")]
    //public async Task<IActionResult> GetByDepartment(int departmentId)
    //    => Ok(await _repo.GetByDepartmentAsync(departmentId));

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repo.GetAllAsync();

        var result = data.Select(c => new CourseDto
        {
            CourseId = c.CourseId,
            DepartmentId = c.DepartmentId,
            DepartmentName = c.Department?.DepartmentName ?? "",
            CourseName = c.CourseName,
            CourseCode = c.CourseCode,
            IsActive = c.IsActive
        }).ToList();

        return Ok(result);
    }

    [HttpGet("by-department/{departmentId}")]
    public async Task<IActionResult> GetByDepartment(int departmentId)
    {
        var data = await _repo.GetByDepartmentAsync(departmentId);

        var result = data.Select(c => new CourseDto
        {
            CourseId = c.CourseId,
            DepartmentId = c.DepartmentId,
            DepartmentName = c.Department?.DepartmentName ?? "",
            CourseName = c.CourseName,
            CourseCode = c.CourseCode,
            IsActive = c.IsActive
        }).ToList();

        return Ok(result);
    }


    [HttpPost]
    //public async Task<IActionResult> Create(Course course)
    //{
    //    await _repo.AddAsync(course);
    //    return Ok();
    //}
    //public async Task<IActionResult> Create(CourseCreateDto dto)
    //{
    //    var course = new Course
    //    {
    //        DepartmentId = dto.DepartmentId,
    //        CourseName = dto.CourseName,
    //        CourseCode = dto.CourseCode,
    //        IsActive = true,
    //        CreatedAt = DateTime.Now
    //    };

    //    await _repo.AddAsync(course);
    //    return Ok();
    //}

    [HttpPost]
    public async Task<IActionResult> Create(CourseCreateDto dto)
    {
        try
        {
            var course = new Course
            {
                DepartmentId = dto.DepartmentId,
                CourseName = dto.CourseName,
                CourseCode = dto.CourseCode,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(course);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);  // ✅ return message
        }
    }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(int id, Course course)
    //{
    //    course.CourseId = id;
    //    await _repo.UpdateAsync(course);
    //    return Ok();
    //}

    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(int id, CourseUpdateDto dto)
    //{
    //    var course = new Course
    //    {
    //        CourseId = id,
    //        DepartmentId = dto.DepartmentId,
    //        CourseName = dto.CourseName,
    //        CourseCode = dto.CourseCode,
    //        IsActive = dto.IsActive
    //    };

    //    await _repo.UpdateAsync(course);
    //    return Ok();
    //}

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CourseUpdateDto dto)
    {
        try
        {
            var course = new Course
            {
                CourseId = id,
                DepartmentId = dto.DepartmentId,
                CourseName = dto.CourseName,
                CourseCode = dto.CourseCode,
                IsActive = dto.IsActive
            };

            await _repo.UpdateAsync(course);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);  // ✅ return message
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        await _repo.ToggleStatusAsync(id);
        return Ok();
    }
}