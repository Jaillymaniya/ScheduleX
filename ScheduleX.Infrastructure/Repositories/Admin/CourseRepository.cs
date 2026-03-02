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

    //public async Task AddAsync(Course course)
    //{
    //    // Check duplicate CourseCode
    //    if (await CourseCodeExistsAsync(course.CourseCode))
    //        throw new Exception("Course already exists.");

    //    course.IsActive = true;
    //    course.CreatedAt = DateTime.Now;

    //    _db.Courses.Add(course);

    //    try
    //    {
    //        await _db.SaveChangesAsync();
    //    }
    //    catch (DbUpdateException ex)
    //    {
    //        // Backup safety (in case race condition)
    //        if (ex.InnerException?.Message.Contains("IX_TblCourse_CourseCode") == true)
    //            throw new Exception("Course already exists.");

    //        throw;
    //    }
    //}


    public async Task AddAsync(Course course)
    {
        if (!string.IsNullOrWhiteSpace(course.CourseCode))
        {
            var code = course.CourseCode.Trim();

            var exists = await _db.Courses.AnyAsync(c => c.CourseCode == code);
            if (exists)
                throw new Exception("Course code already exists.");
        }

        course.IsActive = true;
        course.CreatedAt = DateTime.Now;

        _db.Courses.Add(course);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("IX_TblCourse_CourseCode") == true)
                throw new Exception("Course code already exists.");

            throw;
        }
    }


    //public async Task UpdateAsync(Course course)
    //{
    //    //var existing = await _db.Courses.FirstOrDefaultAsync(x => x.CourseId == course.CourseId);
    //    //if (existing == null) return;

    //    //existing.DepartmentId = course.DepartmentId;
    //    //existing.CourseName = course.CourseName;
    //    //existing.CourseCode = course.CourseCode;

    //    //await _db.SaveChangesAsync();
    //    var existing = await _db.Courses.FirstOrDefaultAsync(x => x.CourseId == course.CourseId);
    //    if (existing == null) return;

    //    // Check duplicate CourseCode (exclude current record)
    //    if (await CourseCodeExistsAsync(course.CourseCode, course.CourseId))
    //        throw new Exception("Course already exists.");

    //    existing.DepartmentId = course.DepartmentId;
    //    existing.CourseName = course.CourseName;
    //    existing.CourseCode = course.CourseCode;

    //    try
    //    {
    //        await _db.SaveChangesAsync();
    //    }
    //    catch (DbUpdateException ex)
    //    {
    //        if (ex.InnerException?.Message.Contains("IX_TblCourse_CourseCode") == true)
    //            throw new Exception("Course already exists.");

    //        throw;
    //    }
    //}


    public async Task UpdateAsync(Course course)
    {
        var existing = await _db.Courses.FirstOrDefaultAsync(x => x.CourseId == course.CourseId);
        if (existing == null)
            throw new Exception("Course not found.");

        // ✅ Duplicate CourseCode check (exclude current record)
        if (!string.IsNullOrWhiteSpace(course.CourseCode))
        {
            var code = course.CourseCode.Trim();

            var exists = await _db.Courses
                .AnyAsync(c => c.CourseCode == code && c.CourseId != course.CourseId);

            if (exists)
                throw new Exception("Course code already exists.");
        }

        existing.DepartmentId = course.DepartmentId;
        existing.CourseName = course.CourseName;
        existing.CourseCode = string.IsNullOrWhiteSpace(course.CourseCode) ? null : course.CourseCode.Trim();
        existing.IsActive = course.IsActive;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // safety if two users update same time
            if (ex.InnerException?.Message.Contains("IX_TblCourse_CourseCode") == true)
                throw new Exception("Course code already exists.");

            throw;
        }
    }


    public async Task ToggleStatusAsync(int courseId)
    {
        var course = await _db.Courses.FirstOrDefaultAsync(x => x.CourseId == courseId);
        if (course == null) return;

        course.IsActive = !course.IsActive;
        await _db.SaveChangesAsync();
    }

    private async Task<bool> CourseCodeExistsAsync(string? code, int excludeCourseId = 0)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;

        code = code.Trim();

        return await _db.Courses
            .AnyAsync(c => c.CourseCode == code && c.CourseId != excludeCourseId);
    }
}