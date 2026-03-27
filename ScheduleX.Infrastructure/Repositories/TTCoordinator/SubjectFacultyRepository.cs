using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Core.Interfaces.TTCoordinator;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator
{
  
    public class SubjectFacultyRepository : ISubjectFacultyRepository
    {
        private readonly AppDbContext _context;

        public SubjectFacultyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetCoursesAsync()
        {
            return await _context.Courses
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<List<Semester>> GetSemesters(int courseId)
        {
            return await _context.Semesters
                .Where(x => x.CourseId == courseId && x.IsActive)
                .ToListAsync();
        }
        public async Task<List<Course>> GetCoursesByCoordinator(int userId)
        {
            return await _context.TTCoordinatorCourses
                .Where(x => x.UserId == userId)
                .Include(x => x.Course)
                .Select(x => x.Course)
                .Where(c => c.IsActive)
                .ToListAsync();
        }
        public async Task<List<Division>> GetDivisions(int semesterId)
        {
            return await _context.Divisions
                .Where(x => x.SemesterId == semesterId && x.IsActive)
                .ToListAsync();
        }

        public async Task<List<SubjectSemester>> GetSubjectSemesters(int semesterId)
        {
            return await _context.SubjectSemesters
                .Include(x => x.Subject)
                .Include(x => x.Semester)
                .Where(x => x.SemesterId == semesterId && x.IsActive)
                .ToListAsync();
        }

        public async Task<List<Faculty>> GetFaculties(int courseId)
        {
            var deptId = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => c.DepartmentId)
                .FirstOrDefaultAsync();

            return await _context.Faculties
                .Where(f => f.DepartmentId == deptId && f.IsActive)
                .ToListAsync();
        }

        // 🔥 NEW METHODS

        public async Task<List<Department>> GetDepartments()
        {
            return await _context.Departments
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByDepartment(int deptId)
        {
            return await _context.Courses
                .Where(x => x.DepartmentId == deptId && x.IsActive)
                .ToListAsync();
        }

        public async Task<List<Faculty>> GetFacultyByCourse(int courseId)
        {
            var deptId = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => c.DepartmentId)
                .FirstOrDefaultAsync();

            return await _context.Faculties
                .Where(f => f.DepartmentId == deptId && f.IsActive)
                .ToListAsync();
        }

        public async Task<List<SubjectFaculty>> GetAllAsync()
        {
            return await _context.SubjectFaculties
                .Include(x => x.SubjectSemester)
                    .ThenInclude(s => s.Subject)
                .Include(x => x.SubjectSemester.Semester)
                    .ThenInclude(c => c.Course)
                .Include(x => x.Division)
                .Include(x => x.Faculty)
                .ToListAsync();
        }

        public async Task<(bool, string)> AddAsync(SubjectFaculty model)
        {
            var exists = await _context.SubjectFaculties.AnyAsync(x =>
                x.SubjectSemesterId == model.SubjectSemesterId &&
                x.DivisionId == model.DivisionId);

            if (exists)
                return (false, "Already assigned for this division");

            _context.SubjectFaculties.Add(model);
            await _context.SaveChangesAsync();

            return (true, "Faculty assigned successfully");
        }
        public async Task<Faculty?> GetFacultyByEmail(string email)
        {
            return await _context.Faculties
                .Where(f => f.Email == email && f.IsActive)
                .FirstOrDefaultAsync();
        }
        public async Task ToggleAsync(int id)
        {
            var data = await _context.SubjectFaculties.FindAsync(id);

            if (data != null)
            {
                data.IsActive = !data.IsActive;
                await _context.SaveChangesAsync();
            }
        }
    }
}
