using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace ScheduleX.Infrastructure.Repositories.TTCoordinator
//{
//    internal class SemesterRepository
//    {
//    }
//}


using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

public class SemesterRepository : ISemesterRepository
{
    private readonly AppDbContext _context;

    public SemesterRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Semester>> GetAllAsync()
    {
        return await _context.Semesters
            .Include(x => x.Course)
            .ThenInclude(c => c.Department)
            .ToListAsync();
    }

    public async Task<List<Semester>> GetByCourseAsync(int courseId)
    {
        return await _context.Semesters
            .Where(x => x.CourseId == courseId)
            .Include(x => x.Course)
            .ToListAsync();
    }

    public async Task AddAsync(Semester semester)
    {
        // Duplicate check
        var exists = await _context.Semesters
            .AnyAsync(x => x.CourseId == semester.CourseId
                        && x.SemesterNo == semester.SemesterNo);

        if (exists)
            throw new Exception("Semester already exists for this course.");

        _context.Semesters.Add(semester);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Semester semester)
    {
        var exists = await _context.Semesters
            .AnyAsync(x => x.CourseId == semester.CourseId
                        && x.SemesterNo == semester.SemesterNo
                        && x.SemesterId != semester.SemesterId);

        if (exists)
            throw new Exception("Duplicate semester for this course.");

        _context.Semesters.Update(semester);
        await _context.SaveChangesAsync();
    }

    public async Task ToggleStatusAsync(int id)
    {
        var semester = await _context.Semesters.FindAsync(id);
        if (semester != null)
        {
            semester.IsActive = !semester.IsActive;
            await _context.SaveChangesAsync();
        }
    }
}