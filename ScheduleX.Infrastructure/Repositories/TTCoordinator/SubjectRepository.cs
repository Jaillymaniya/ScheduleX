using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task<List<Subject>> GetAllAsync()
        {
            return await _context.Subjects
                .Include(s => s.Course)
               
                .OrderByDescending(s => s.IsActive)
                .ToListAsync();
        }

        
        public async Task<List<Course>> GetCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<bool> IsSubjectCodeExists(string code, int? id = null)
        {
            return await _context.Subjects
                .AnyAsync(s => s.SubjectCode == code && (id == null || s.SubjectId != id));
        }

        public async Task<(bool, string)> AddAsync(Subject subject)
        {
            if (await IsSubjectCodeExists(subject.SubjectCode))
                return (false, "Duplicate Subject Code");

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return (true, "Subject Added");
        }

        public async Task<(bool, string)> UpdateAsync(Subject subject)
        {
            if (await IsSubjectCodeExists(subject.SubjectCode, subject.SubjectId))
                return (false, "Duplicate Subject Code");

            var existing = await _context.Subjects.FindAsync(subject.SubjectId);

            if (existing != null)
            {
                existing.SubjectName = subject.SubjectName;
                existing.SubjectCode = subject.SubjectCode;
                existing.CourseId = subject.CourseId; 
                existing.Credits = subject.Credits;
                existing.SubjectCategory = subject.SubjectCategory;
                existing.IsElective = subject.IsElective;

                await _context.SaveChangesAsync();
            }

            return (true, "Updated Successfully");
        }

        public async Task SoftDeleteAsync(int id)
        {
            var s = await _context.Subjects.FindAsync(id);
            if (s != null)
            {
                s.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivateAsync(int id)
        {
            var s = await _context.Subjects.FindAsync(id);
            if (s != null)
            {
                s.IsActive = true;
                await _context.SaveChangesAsync();
            }
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
        //public async Task<(bool, string)> BulkInsertAsync(List<Subject> subjects)
        //{
        //    var courses = await _context.Courses.ToListAsync();

        //    int row = 1;

        //    foreach (var s in subjects)
        //    {
        //        row++;

        //        // ❌ Subject Name EMPTY
        //        if (string.IsNullOrWhiteSpace(s.SubjectName))
        //            return (false, $"Row {row}: Subject Name is required");

        //        // ❌ Subject Code EMPTY
        //        if (string.IsNullOrWhiteSpace(s.SubjectCode))
        //            return (false, $"Row {row}: Subject Code is required");

        //        // ❌ CourseId not set
        //        if (s.CourseId == 0)
        //            return (false, $"Row {row}: Course is required");

        //        // ❌ INVALID COURSE ID
        //        var courseExists = courses.Any(c => c.CourseId == s.CourseId);

        //        if (!courseExists)
        //            return (false, $"Row {row}: Invalid CourseId '{s.CourseId}'");

        //        // ❌ DUPLICATE CODE
        //        if (await IsSubjectCodeExists(s.SubjectCode))
        //            return (false, $"Row {row}: Duplicate Code '{s.SubjectCode}'");

        //        // ✅ IMPORTANT: prevent EF inserting Course
        //        s.Course = null;
        //    }

        //    await _context.Subjects.AddRangeAsync(subjects);
        //    await _context.SaveChangesAsync();

        //    return (true, "CSV Uploaded Successfully");
        //}

        public async Task<(bool, string)> BulkInsertAsync(List<Subject> subjects, int userId)
        {
            var allowedCourses = await _context.TTCoordinatorCourses
                .Where(x => x.UserId == userId && x.Course.IsActive)
                .Select(x => x.CourseId)
                .ToListAsync();

            int row = 1;

            foreach (var s in subjects)
            {
                row++;

                if (string.IsNullOrWhiteSpace(s.SubjectName))
                    return (false, $"Row {row}: Subject Name is required");

                if (string.IsNullOrWhiteSpace(s.SubjectCode))
                    return (false, $"Row {row}: Subject Code is required");

                if (!allowedCourses.Contains(s.CourseId))
                    return (false, $"Row {row}: Course not allowed for you");

                if (await IsSubjectCodeExists(s.SubjectCode))
                    return (false, $"Row {row}: Duplicate Code '{s.SubjectCode}'");

                s.Course = null;
            }

            await _context.Subjects.AddRangeAsync(subjects);
            await _context.SaveChangesAsync();

            return (true, "CSV Uploaded Successfully");
        }
    }
}
