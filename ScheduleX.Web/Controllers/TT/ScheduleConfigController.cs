using Microsoft.AspNetCore.Mvc;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Web.DTOs;




namespace ScheduleX.Web.Controllers.TT;

[ApiController]
[Route("api/tt/schedule-config")]
public class ScheduleConfigController : ControllerBase
{
    private readonly IScheduleConfigRepository _repo;

    public ScheduleConfigController(IScheduleConfigRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("by-course/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var data = await _repo.GetByCourseAsync(courseId);
        if (data == null) return Ok(null);

        var dto = new ScheduleConfigDto
        {
            ConfigId = data.ConfigId,
            DepartmentId = data.DepartmentId,
            CourseId = data.CourseId,
            CourseName = data.Course?.CourseName,
            StartTime = data.StartTime.ToString("HH:mm"),
            EndTime = data.EndTime.ToString("HH:mm"),
            LectureDurationMin = data.LectureDurationMin,
            WorkingDaysMask = data.WorkingDaysMask,
            LecturesPerDay = data.LecturesPerDay,
            IsActive = data.IsActive,
            BreakRules = data.BreakRules.OrderBy(x => x.BreakNo).Select(x => new BreakRuleDto
            {
                BreakRuleId = x.BreakRuleId,
                ConfigId = x.ConfigId,
                BreakNo = x.BreakNo,
                AfterLectureNo = x.AfterLectureNo,
                BreakDurationMin = x.BreakDurationMin,
                BreakName = x.BreakName
            }).ToList()
        };

        return Ok(dto);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] ScheduleConfigDto dto)
    {
        if (!TimeOnly.TryParse(dto.StartTime, out var start))
            return BadRequest("Invalid start time.");

        if (!TimeOnly.TryParse(dto.EndTime, out var end))
            return BadRequest("Invalid end time.");

        if (start >= end)
            return BadRequest("Start time must be less than end time.");

        if (dto.LectureDurationMin <= 0)
            return BadRequest("Lecture duration must be greater than 0.");

        if (dto.LecturesPerDay <= 0)
            return BadRequest("Lectures per day must be greater than 0.");

        if (dto.WorkingDaysMask <= 0)
            return BadRequest("Select at least one working day.");
        if (dto.DepartmentId <= 0)
            return BadRequest("Invalid DepartmentId.");

        if (dto.CourseId == null || dto.CourseId <= 0)
            return BadRequest("Invalid CourseId.");

        var entity = new ScheduleConfig
        {
            ConfigId = dto.ConfigId,
            DepartmentId = dto.DepartmentId,
            CourseId = dto.CourseId,
            StartTime = start,
            EndTime = end,
            LectureDurationMin = dto.LectureDurationMin,
            WorkingDaysMask = dto.WorkingDaysMask,
            LecturesPerDay = dto.LecturesPerDay,
            IsActive = dto.IsActive
        };

        var saved = await _repo.SaveAsync(entity);

        dto.ConfigId = saved.ConfigId;
        return Ok(dto);
    }

    [HttpGet("breaks/{configId}")]
    public async Task<IActionResult> GetBreaks(int configId)
    {
        var data = await _repo.GetBreakRulesAsync(configId);

        var result = data.Select(x => new BreakRuleDto
        {
            BreakRuleId = x.BreakRuleId,
            ConfigId = x.ConfigId,
            BreakNo = x.BreakNo,
            AfterLectureNo = x.AfterLectureNo,
            BreakDurationMin = x.BreakDurationMin,
            BreakName = x.BreakName
        }).ToList();

        return Ok(result);
    }

    [HttpPost("breaks")]
    public async Task<IActionResult> AddBreak([FromBody] BreakRuleDto dto)
    {
        if (dto.ConfigId <= 0) return BadRequest("Save config first.");
        if (dto.BreakNo <= 0) return BadRequest("Break no is required.");
        if (dto.AfterLectureNo <= 0) return BadRequest("After lecture no is required.");
        if (dto.BreakDurationMin <= 0) return BadRequest("Break duration must be greater than 0.");

        var existingBreaks = await _repo.GetBreakRulesAsync(dto.ConfigId);
        if (existingBreaks.Any(x => x.AfterLectureNo == dto.AfterLectureNo))
            return BadRequest("Break after this lecture already exists.");

        var entity = new BreakRule
        {
            ConfigId = dto.ConfigId,
            BreakNo = dto.BreakNo,
            AfterLectureNo = dto.AfterLectureNo,
            BreakDurationMin = dto.BreakDurationMin,
            BreakName = dto.BreakName
        };

        var saved = await _repo.AddBreakRuleAsync(entity);
        dto.BreakRuleId = saved.BreakRuleId;
        return Ok(dto);
    }

    [HttpPut("breaks/{id}")]
    public async Task<IActionResult> UpdateBreak(int id, [FromBody] BreakRuleDto dto)
    {
        if (id != dto.BreakRuleId) return BadRequest("Invalid break id.");

        var entity = new BreakRule
        {
            BreakRuleId = dto.BreakRuleId,
            ConfigId = dto.ConfigId,
            BreakNo = dto.BreakNo,
            AfterLectureNo = dto.AfterLectureNo,
            BreakDurationMin = dto.BreakDurationMin,
            BreakName = dto.BreakName
        };

        var updated = await _repo.UpdateBreakRuleAsync(entity);
        if (updated == null) return NotFound();

        return Ok(dto);
    }

    [HttpDelete("breaks/{id}")]
    public async Task<IActionResult> DeleteBreak(int id)
    {
        var ok = await _repo.DeleteBreakRuleAsync(id);
        if (!ok) return NotFound();
        return Ok();
    }

    [HttpPost("generate-slots/{configId}")]
    public async Task<IActionResult> GenerateSlots(int configId)
    {
        var data = await _repo.GenerateTimeSlotsAsync(configId);

        var result = data.Select(x => new TimeSlotDto
        {
            TimeSlotId = x.TimeSlotId,
            ConfigId = x.ConfigId,
            SlotNo = x.SlotNo,
            StartTime = x.StartTime.ToString("hh:mm tt"),
            EndTime = x.EndTime.ToString("hh:mm tt"),
            SlotType = x.SlotType,
            BreakRuleId = x.BreakRuleId,
            BreakName = x.BreakRule?.BreakName
        }).ToList();

        return Ok(result);
    }

    [HttpGet("slots/{configId}")]
    public async Task<IActionResult> GetSlots(int configId)
    {
        var data = await _repo.GetTimeSlotsAsync(configId);

        var result = data.Select(x => new TimeSlotDto
        {
            TimeSlotId = x.TimeSlotId,
            ConfigId = x.ConfigId,
            SlotNo = x.SlotNo,
            StartTime = x.StartTime.ToString("hh:mm tt"),
            EndTime = x.EndTime.ToString("hh:mm tt"),
            SlotType = x.SlotType,
            BreakRuleId = x.BreakRuleId,
            BreakName = x.BreakRule?.BreakName
        }).ToList();

        return Ok(result);
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates()
    {
        var data = await _repo.GetActiveTemplatesAsync();

        var result = data.Select(x => new TemplateDto
        {
            TemplateId = x.TemplateId,
            TemplateName = x.TemplateName,
            LayoutType = x.LayoutType,
            IsDefault = x.IsDefault
        }).ToList();

        return Ok(result);
    }
}