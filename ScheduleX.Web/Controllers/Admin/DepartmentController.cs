using Microsoft.AspNetCore.Mvc;
using ScheduleX.Core.Interfaces;
using ScheduleX.Core.Entities;

namespace ScheduleX.Web.Controllers.Admin;

[ApiController]
[Route("api/admin/department")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentRepository _repository;

    public DepartmentController(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _repository.GetAllAsync());

    //[HttpPost]
    //public async Task<IActionResult> Create(Department department)
    //{
    //    await _repository.AddAsync(department);
    //    return Ok();
    //}
    [HttpPost]
    public async Task<IActionResult> Create(Department department)
    {
        try
        {
            await _repository.AddAsync(department);
            return Ok(new { success = true, message = "Department added successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = GetErrorMessage(ex) });
        }
    }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(int id, Department department)
    //{
    //    await _repository.UpdateAsync(department);
    //    return Ok();
    //}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Department department)
    {
        try
        {
            await _repository.UpdateAsync(department);
            return Ok(new { success = true, message = "Department updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = GetErrorMessage(ex) });
        }
    }

    //[HttpPatch("{id}")]
    //public async Task<IActionResult> Toggle(int id)
    //{
    //    await _repository.ToggleStatusAsync(id);
    //    return Ok();
    //}
    [HttpPatch("{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        try
        {
            await _repository.ToggleStatusAsync(id);
            return Ok(new { success = true, message = "Status updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = GetErrorMessage(ex) });
        }
    }

    private string GetErrorMessage(Exception ex)
    {
        if (ex.InnerException?.Message.Contains("IX_TblDepartment_DepartmentCode") == true)
            return "Department Code already exists";

        if (ex.InnerException?.Message.Contains("IX_TblDepartment_DepartmentName") == true)
            return "Department Name already exists";

        return "Something went wrong. Please try again.";
    }
}

