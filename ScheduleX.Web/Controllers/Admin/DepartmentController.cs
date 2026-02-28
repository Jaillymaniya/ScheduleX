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

    [HttpPost]
    public async Task<IActionResult> Create(Department department)
    {
        await _repository.AddAsync(department);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Department department)
    {
        await _repository.UpdateAsync(department);
        return Ok();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        await _repository.ToggleStatusAsync(id);
        return Ok();
    }
}