using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

public class SubjectSemesterRepository : ISubjectSemesterRepository
{
    private readonly AppDbContext _context;

    public SubjectSemesterRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Course>> GetCoursesAsync(int departmentId)
    {
        return await _context.Courses
            .Where(c => c.DepartmentId == departmentId && c.IsActive)
            .ToListAsync();
    }

    public async Task<List<Semester>> GetAllSemestersAsync()
    {
        return await _context.Semesters
            .Where(s => s.IsActive)
            .ToListAsync();
    }

    public async Task<List<Subject>> GetAllSubjectsAsync()
    {
        return await _context.Subjects
            .Where(s => s.IsActive)
            .ToListAsync();
    }
    public async Task<List<Semester>> GetSemestersByCourse(int courseId)
    {
        return await _context.Semesters
            .Where(s => s.CourseId == courseId && s.IsActive)
            .ToListAsync();
    }
    public async Task UpdateAsync(SubjectSemester model)
    {
        var existing = await _context.SubjectSemesters.FindAsync(model.SubjectSemesterId);

        if (existing != null)
        {
            existing.SubjectId = model.SubjectId;
            existing.SemesterId = model.SemesterId;

            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<Subject>> GetSubjectsByCourse(int courseId)
    {
        return await _context.Subjects
            .Where(s => s.CourseId == courseId && s.IsActive)
            .ToListAsync();
    }

    public async Task<List<SubjectSemester>> GetAllAsync()
    {
        return await _context.SubjectSemesters
            .Include(x => x.Subject)
            .Include(x => x.Semester)
                .ThenInclude(s => s.Course)
            .OrderByDescending(x => x.IsActive)   // ✅ Active first
            .ThenBy(x => x.Semester.SemesterNo)   // optional sorting
            .ToListAsync();
    }
    public async Task<List<Course>> GetCoursesForCoordinatorAsync(int userId)
    {
        return await _context.TTCoordinatorCourses
            .Include(x => x.Course)
            .Where(x => x.UserId == userId && x.Course.IsActive)
            .Select(x => x.Course)
            .Distinct()
            .ToListAsync();
    }
    public async Task<(bool, string)> AddAsync(SubjectSemester model)
    {
        var exists = await _context.SubjectSemesters
            .AnyAsync(x => x.SubjectId == model.SubjectId &&
                           x.SemesterId == model.SemesterId);

        if (exists)
            return (false, "Duplicate mapping");
        // prevent cross-course insert
        var subject = await _context.Subjects.FindAsync(model.SubjectId);
        var semester = await _context.Semesters.FindAsync(model.SemesterId);

        if (subject.CourseId != semester.CourseId)
            return (false, "Subject and Semester must belong to same course");
        _context.SubjectSemesters.Add(model);
        await _context.SaveChangesAsync();

        return (true, "Added successfully");
    }

    public async Task<(bool, string)> BulkInsertAsync(List<SubjectSemester> list)
    {
        foreach (var item in list)
        {
            var exists = await _context.SubjectSemesters
                .AnyAsync(x => x.SubjectId == item.SubjectId &&
                               x.SemesterId == item.SemesterId);

            if (exists)
                return (false, "Duplicate in CSV");
        }

        await _context.SubjectSemesters.AddRangeAsync(list);
        await _context.SaveChangesAsync();

        return (true, "CSV Uploaded Successfully");
    }

    public async Task SoftDeleteAsync(int id)
    {
        var data = await _context.SubjectSemesters.FindAsync(id);

        if (data != null)
        {
            data.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<SubjectSemester>> GetByCoordinatorAsync(int userId)
    {
        return await _context.SubjectSemesters
            .Include(x => x.Subject)
            .Include(x => x.Semester)
                .ThenInclude(s => s.Course)
            .Where(x => _context.TTCoordinatorCourses
                .Any(tc => tc.UserId == userId &&
                           tc.CourseId == x.Semester.CourseId &&
                           tc.Course.IsActive))
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Semester.SemesterNo)
            .ToListAsync();
    }
    public async Task ActivateAsync(int id)
    {
        var data = await _context.SubjectSemesters.FindAsync(id);

        if (data != null)
        {
            data.IsActive = true;
            await _context.SaveChangesAsync();
        }
    }
}