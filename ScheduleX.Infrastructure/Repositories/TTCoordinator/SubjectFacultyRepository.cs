using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;



namespace ScheduleX.Infrastructure.Repositories.TTCoordinator
{
    public class SubjectFacultyRepository : ISubjectFacultyRepository
        {
            private readonly AppDbContext _context;

            public SubjectFacultyRepository(AppDbContext context)
            {
                _context = context;
            }

            // ================= COURSES =================

            public async Task<List<Course>> GetCoursesByCoordinator(int userId)
            {
                return await _context.TTCoordinatorCourses
                    .Where(x => x.UserId == userId)
                    .Include(x => x.Course)
                    .Select(x => x.Course)
                    .Where(c => c.IsActive)
                    .Distinct()
                    .ToListAsync();
            }

            // ================= SEMESTER =================

            public async Task<List<Semester>> GetSemesters(int courseId)
            {
                return await _context.Semesters
                    .Where(x => x.CourseId == courseId && x.IsActive)
                    .ToListAsync();
            }

            // ================= DIVISION =================

            public async Task<List<Division>> GetDivisions(int semesterId)
            {
                return await _context.Divisions
                    .Where(x => x.SemesterId == semesterId && x.IsActive)
                    .ToListAsync();
            }

            // ================= SUBJECT =================

            public async Task<List<SubjectSemester>> GetSubjectSemesters(int semesterId)
            {
                return await _context.SubjectSemesters
                    .Include(x => x.Subject)
                    .Where(x => x.SemesterId == semesterId && x.IsActive)
                    .ToListAsync();
            }

            // ================= FACULTY =================

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

            // ================= CROSS DEPARTMENT =================

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

            public async Task<Faculty?> GetFacultyByEmail(string email)
            {
                return await _context.Faculties
                    .FirstOrDefaultAsync(f => f.Email == email && f.IsActive);
            }

            // ================= GET ALL =================

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

            // ================= ADD =================

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

            // ================= UPDATE =================

            public async Task<(bool, string)> UpdateAsync(SubjectFaculty model)
            {
                var existing = await _context.SubjectFaculties
                    .FirstOrDefaultAsync(x => x.SubjectFacultyId == model.SubjectFacultyId);

                if (existing == null)
                    return (false, "Record not found");

                var duplicate = await _context.SubjectFaculties.AnyAsync(x =>
                    x.SubjectSemesterId == model.SubjectSemesterId &&
                    x.DivisionId == model.DivisionId &&
                    x.SubjectFacultyId != model.SubjectFacultyId);

                if (duplicate)
                    return (false, "Already assigned");

                existing.SubjectSemesterId = model.SubjectSemesterId;
                existing.DivisionId = model.DivisionId;
                existing.FacultyId = model.FacultyId;

                await _context.SaveChangesAsync();

                return (true, "Updated successfully");
            }

            // ================= TOGGLE =================

            public async Task ToggleAsync(int id)
            {
                var data = await _context.SubjectFaculties.FindAsync(id);

                if (data != null)
                {
                    data.IsActive = !data.IsActive;
                    await _context.SaveChangesAsync();
                }
            }

            // ================= BULK INSERT (MANUAL) =================

            public async Task<(bool, string)> BulkInsertAsync(List<SubjectFaculty> list, int userId)
            {
                foreach (var item in list)
                {
                    var ss = await _context.SubjectSemesters
                        .Include(x => x.Semester)
                        .FirstOrDefaultAsync(x => x.SubjectSemesterId == item.SubjectSemesterId);

                    if (ss == null)
                        return (false, "Invalid Subject");

                    var allowed = await _context.TTCoordinatorCourses
                        .AnyAsync(x => x.UserId == userId && x.CourseId == ss.Semester.CourseId);

                    if (!allowed)
                        return (false, "Unauthorized Course");

                    var exists = await _context.SubjectFaculties.AnyAsync(x =>
                        x.SubjectSemesterId == item.SubjectSemesterId &&
                        x.DivisionId == item.DivisionId);

                    if (exists)
                        return (false, "Duplicate Entry");
                }

                await _context.SubjectFaculties.AddRangeAsync(list);
                await _context.SaveChangesAsync();

                return (true, "CSV Uploaded Successfully");
            }


    }
    }

