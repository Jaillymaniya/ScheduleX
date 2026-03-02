//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ScheduleX.Infrastructure.Repositories.Admin
//{
//    internal class CourseRepository
//    {
//    }
//}

using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;
using ScheduleX.Infrastructure.Data;

namespace Timetable.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;
    public CourseRepository(AppDbContext db) => _db = db;

    public async Task<List<Course>> GetAllAsync()
        => await _db.Courses
            .Include(c => c.Department)
            .OrderBy(c => c.Department.DepartmentName)
            .ThenBy(c => c.CourseName)
            .ToListAsync();

    public async Task<List<Course>> GetByDepartmentAsync(int departmentId)
        => await _db.Courses
            .Include(c => c.Department)
            .Where(c => c.DepartmentId == departmentId)
            .OrderBy(c => c.CourseName)
            .ToListAsync();

    public async Task AddAsync(Course course)
    {
        course.IsActive = true;
        course.CreatedAt = DateTime.Now;
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        var existing = await _db.Courses.FirstOrDefaultAsync(x => x.CourseId == course.CourseId);
        if (existing == null) return;

        existing.DepartmentId = course.DepartmentId;
        existing.CourseName = course.CourseName;
        existing.CourseCode = course.CourseCode;

        await _db.SaveChangesAsync();
    }

    public async Task ToggleStatusAsync(int courseId)
    {
        var course = await _db.Courses.FirstOrDefaultAsync(x => x.CourseId == courseId);
        if (course == null) return;

        course.IsActive = !course.IsActive;
        await _db.SaveChangesAsync();
    }
}