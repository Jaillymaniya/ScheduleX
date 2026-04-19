using Microsoft.AspNetCore.Mvc;
using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Web.DTOs;
using Microsoft.EntityFrameworkCore;


namespace ScheduleX.Web.Controllers.TT;

[Route("api/tt/templates")]
[ApiController]
public class TimeTableTemplateController : ControllerBase
{
    private readonly AppDbContext _context;

    public TimeTableTemplateController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeTableTemplateDto>>> GetAll()
    {
        var templates = await _context.TimeTableTemplates
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.TemplateName)
            .Select(x => new TimeTableTemplateDto
            {
                TemplateId = x.TemplateId,
                TemplateName = x.TemplateName,
                LayoutType = (byte)x.LayoutType,
                LayoutTypeName = x.LayoutType.ToString(),
                TemplateJson = x.TemplateJson,
                IsDefault = x.IsDefault,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                //UsedInBatchCount = x.TimeTableBatches.Count()
                UsedInBatchCount = _context.TimeTableBatches
    .Count(b => b.TemplateId == x.TemplateId)
            })
            .ToListAsync();

        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TimeTableTemplateDto>> GetById(int id)
    {
        var template = await _context.TimeTableTemplates
            .Where(x => x.TemplateId == id)
            .Select(x => new TimeTableTemplateDto
            {
                TemplateId = x.TemplateId,
                TemplateName = x.TemplateName,
                LayoutType = (byte)x.LayoutType,
                LayoutTypeName = x.LayoutType.ToString(),
                TemplateJson = x.TemplateJson,
                IsDefault = x.IsDefault,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                //UsedInBatchCount = x.TimeTableBatches.Count()
                UsedInBatchCount = _context.TimeTableBatches
    .Count(b => b.TemplateId == x.TemplateId)
            })
            .FirstOrDefaultAsync();

        if (template == null)
            return NotFound("Template not found.");

        return Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateTimeTableTemplateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var nameExists = await _context.TimeTableTemplates
            .AnyAsync(x => x.TemplateName.Trim().ToLower() == dto.TemplateName.Trim().ToLower());

        if (nameExists)
            return BadRequest("Template name already exists.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (dto.IsDefault)
            {
                var oldDefaults = await _context.TimeTableTemplates
                    .Where(x => x.IsDefault)
                    .ToListAsync();

                foreach (var old in oldDefaults)
                    old.IsDefault = false;
            }

            var template = new TimeTableTemplate
            {
                TemplateName = dto.TemplateName.Trim(),
                LayoutType = (LayoutTypeEnum)dto.LayoutType,
                TemplateJson = dto.TemplateJson,
                IsDefault = dto.IsDefault,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.TimeTableTemplates.Add(template);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Template created successfully." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Error creating template: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTimeTableTemplateDto dto)
    {
        if (id != dto.TemplateId)
            return BadRequest("Template id mismatch.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var template = await _context.TimeTableTemplates.FirstOrDefaultAsync(x => x.TemplateId == id);
        if (template == null)
            return NotFound("Template not found.");

        var duplicateName = await _context.TimeTableTemplates
            .AnyAsync(x => x.TemplateId != id &&
                           x.TemplateName.Trim().ToLower() == dto.TemplateName.Trim().ToLower());

        if (duplicateName)
            return BadRequest("Another template with same name already exists.");

        template.TemplateName = dto.TemplateName.Trim();
        template.LayoutType = (LayoutTypeEnum)dto.LayoutType;
        template.TemplateJson = dto.TemplateJson;
        template.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Template updated successfully." });
    }

    [HttpPut("{id}/set-default")]
    public async Task<ActionResult> SetDefault(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var template = await _context.TimeTableTemplates.FirstOrDefaultAsync(x => x.TemplateId == id);
            if (template == null)
                return NotFound("Template not found.");

            if (!template.IsActive)
                return BadRequest("Inactive template cannot be selected as default.");

            var allTemplates = await _context.TimeTableTemplates.ToListAsync();
            foreach (var item in allTemplates)
                item.IsDefault = false;

            template.IsDefault = true;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Template marked as default successfully." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Error while setting default template: {ex.Message}");
        }
    }

    [HttpPut("{id}/soft-delete")]
    public async Task<ActionResult> SoftDelete(int id)
    {
        var template = await _context.TimeTableTemplates
            .Include(x => x.TimeTableBatches)
            .FirstOrDefaultAsync(x => x.TemplateId == id);

        if (template == null)
            return NotFound("Template not found.");

        template.IsActive = false;
        template.IsDefault = false;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Template soft deleted successfully." });
    }

    [HttpPut("{id}/restore")]
    public async Task<ActionResult> Restore(int id)
    {
        var template = await _context.TimeTableTemplates.FirstOrDefaultAsync(x => x.TemplateId == id);

        if (template == null)
            return NotFound("Template not found.");

        template.IsActive = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Template restored successfully." });
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<TimeTableTemplateDto>>> GetActiveTemplates()
    {
        var templates = await _context.TimeTableTemplates
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.TemplateName)
            .Select(x => new TimeTableTemplateDto
            {
                TemplateId = x.TemplateId,
                TemplateName = x.TemplateName,
                LayoutType = (byte)x.LayoutType,
                LayoutTypeName = x.LayoutType.ToString(),
                TemplateJson = x.TemplateJson,
                IsDefault = x.IsDefault,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                //UsedInBatchCount = x.TimeTableBatches.Count()
                UsedInBatchCount = _context.TimeTableBatches
    .Count(b => b.TemplateId == x.TemplateId)
            })
            .ToListAsync();

        return Ok(templates);
    }
}