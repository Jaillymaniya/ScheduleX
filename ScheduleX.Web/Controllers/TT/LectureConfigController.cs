using Microsoft.AspNetCore.Mvc;
using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Web.DTOs;
using Microsoft.EntityFrameworkCore;




namespace ScheduleX.Web.Controllers.TT;

[ApiController]
[Route("api/tt/lectureconfig")]
public class LectureConfigController : ControllerBase
{
    private readonly AppDbContext _context;

    public LectureConfigController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("semesters/{userId:int}")]
    public async Task<IActionResult> GetSemesters(int userId)
    {
        var assignedCourseId = await _context.TTCoordinatorCourses
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.CourseId)
            .FirstOrDefaultAsync();

        if (!assignedCourseId.HasValue)
            return NotFound("No course assigned to this timetable coordinator.");

        var semesters = await _context.Semesters
            .Where(x => x.CourseId == assignedCourseId.Value && x.IsActive)
            .OrderBy(x => x.SemesterNo)
            .Select(x => new LectureConfigSemesterDto
            {
                SemesterId = x.SemesterId,
                SemesterNo = x.SemesterNo
            })
            .ToListAsync();

        return Ok(semesters);
    }

    [HttpGet("rows/{semesterId:int}")]
    public async Task<IActionResult> GetRows(int semesterId, [FromQuery] int userId)
    {
        var assignedCourseId = await _context.TTCoordinatorCourses
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.CourseId)
            .FirstOrDefaultAsync();

        if (!assignedCourseId.HasValue)
            return NotFound("No course assigned to this timetable coordinator.");

        var semester = await _context.Semesters
            .FirstOrDefaultAsync(x => x.SemesterId == semesterId && x.IsActive);

        if (semester == null)
            return NotFound("Semester not found.");

        if (semester.CourseId != assignedCourseId.Value)
            return BadRequest("This semester does not belong to your assigned course.");

        var rows = await _context.SubjectSemesters
            .Where(ss => ss.SemesterId == semesterId && ss.IsActive)
            .Include(ss => ss.Subject)
            .Include(ss => ss.LectureConfigs.Where(lc => lc.IsActive))
            .OrderBy(ss => ss.Subject.SubjectName)
            .Select(ss => new LectureConfigRowDto
            {
                SubjectLectureConfigId = ss.LectureConfigs
                    .Where(lc => lc.IsActive)
                    .Select(lc => lc.SubjectLectureConfigId)
                    .FirstOrDefault(),

                SubjectSemesterId = ss.SubjectSemesterId,
                SubjectId = ss.SubjectId,
                SubjectName = ss.Subject.SubjectName,
                SubjectCode = ss.Subject.SubjectCode,

                TheoryCredits = ss.Subject.TheoryCredits,
                PracticalCredits = ss.Subject.PracticalCredits,

                SubjectCategory = (int)ss.Subject.SubjectCategory,
                SubjectCategoryName = ss.Subject.SubjectCategory.ToString(),

                TheoryLecturesPerWeek = ss.Subject.TheoryCredits,
                PracticalLecturesPerWeek = ss.Subject.PracticalCredits,

                PracticalBlockSize = ss.LectureConfigs
                    .Where(lc => lc.IsActive)
                    .Select(lc => (int?)lc.PracticalBlockSize)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(rows);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] SaveLectureConfigRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.Rows == null || request.Rows.Count == 0)
            return BadRequest("No lecture configuration rows found.");

        var assignedCourseId = await _context.TTCoordinatorCourses
            .Where(x => x.UserId == request.UserId)
            .Select(x => (int?)x.CourseId)
            .FirstOrDefaultAsync();

        if (!assignedCourseId.HasValue)
            return NotFound("No course assigned to this timetable coordinator.");

        var semester = await _context.Semesters
            .FirstOrDefaultAsync(x => x.SemesterId == request.SemesterId && x.IsActive);

        if (semester == null)
            return NotFound("Semester not found.");

        if (semester.CourseId != assignedCourseId.Value)
            return BadRequest("This semester does not belong to your assigned course.");

        var subjectSemesterIds = request.Rows.Select(x => x.SubjectSemesterId).Distinct().ToList();

        var subjectSemesters = await _context.SubjectSemesters
            .Where(ss => subjectSemesterIds.Contains(ss.SubjectSemesterId) && ss.IsActive)
            .Include(ss => ss.Subject)
            .Include(ss => ss.LectureConfigs)
            .ToListAsync();

        foreach (var row in request.Rows)
        {
            var subjectSemester = subjectSemesters.FirstOrDefault(x => x.SubjectSemesterId == row.SubjectSemesterId);
            if (subjectSemester == null)
                return BadRequest($"Invalid SubjectSemesterId: {row.SubjectSemesterId}");

            if (subjectSemester.SemesterId != request.SemesterId)
                return BadRequest($"Subject '{subjectSemester.Subject.SubjectName}' does not belong to selected semester.");

            var theoryPerWeek = subjectSemester.Subject.TheoryCredits;
            var practicalPerWeek = subjectSemester.Subject.PracticalCredits;

            var validationError = ValidateLectureConfig(
                subjectSemester.Subject,
                theoryPerWeek,
                practicalPerWeek,
                row.PracticalBlockSize);

            if (!string.IsNullOrWhiteSpace(validationError))
                return BadRequest(validationError);

            var existing = subjectSemester.LectureConfigs.FirstOrDefault(x => x.IsActive);

            if (existing == null)
            {
                existing = new SubjectLectureConfig
                {
                    SubjectSemesterId = row.SubjectSemesterId,
                    TheoryLecturesPerWeek = (byte)theoryPerWeek,
                    PracticalLecturesPerWeek = (byte)practicalPerWeek,
                    PracticalBlockSize = row.PracticalBlockSize.HasValue ? (byte?)row.PracticalBlockSize.Value : null,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.SubjectLectureConfigs.Add(existing);
            }
            else
            {
                existing.TheoryLecturesPerWeek = (byte)theoryPerWeek;
                existing.PracticalLecturesPerWeek = (byte)practicalPerWeek;
                existing.PracticalBlockSize = row.PracticalBlockSize.HasValue ? (byte?)row.PracticalBlockSize.Value : null;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Lecture configuration saved successfully." });
    }

    private static string? ValidateLectureConfig(
    Subject subject,
    int theoryLecturesPerWeek,
    int practicalLecturesPerWeek,
    int? practicalBlockSize)
    {
        switch (subject.SubjectCategory)
        {
            case SubjectCategoryEnum.Theory:
                if (theoryLecturesPerWeek <= 0)
                    return $"Theory subject '{subject.SubjectName}' must have theory credits greater than 0.";

                if (practicalLecturesPerWeek != 0)
                    return $"Theory subject '{subject.SubjectName}' cannot have practical credits.";

                if (practicalBlockSize.HasValue)
                    return $"Theory subject '{subject.SubjectName}' cannot have practical block size.";
                break;

            case SubjectCategoryEnum.Practical:
                if (theoryLecturesPerWeek != 0)
                    return $"Practical subject '{subject.SubjectName}' cannot have theory credits.";

                if (practicalLecturesPerWeek <= 0)
                    return $"Practical subject '{subject.SubjectName}' must have practical credits greater than 0.";

                if (!practicalBlockSize.HasValue)
                    return $"Practical block size is required for subject '{subject.SubjectName}'.";

                if (practicalLecturesPerWeek % practicalBlockSize.Value != 0)
                    return $"Practical credits must be divisible by block size for subject '{subject.SubjectName}'.";
                break;

            case SubjectCategoryEnum.Both:
                if (theoryLecturesPerWeek <= 0 && practicalLecturesPerWeek <= 0)
                    return $"Subject '{subject.SubjectName}' must have theory or practical credits.";

                if (practicalLecturesPerWeek > 0)
                {
                    if (!practicalBlockSize.HasValue)
                        return $"Practical block size is required for subject '{subject.SubjectName}'.";

                    if (practicalLecturesPerWeek % practicalBlockSize.Value != 0)
                        return $"Practical credits must be divisible by block size for subject '{subject.SubjectName}'.";
                }
                break;
        }

        return null;
    }
}