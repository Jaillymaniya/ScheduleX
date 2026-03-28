using ScheduleX.Core.Entities;


public interface ISubjectFacultyRepository
{
    // ================= DROPDOWNS =================

    Task<List<Course>> GetCoursesByCoordinator(int userId);

    Task<List<Semester>> GetSemesters(int courseId);

    Task<List<Division>> GetDivisions(int semesterId);

    Task<List<SubjectSemester>> GetSubjectSemesters(int semesterId);

    Task<List<Faculty>> GetFaculties(int courseId);

    // ================= CROSS-DEPARTMENT =================

    Task<List<Department>> GetDepartments();

    Task<List<Course>> GetCoursesByDepartment(int deptId);

    Task<List<Faculty>> GetFacultyByCourse(int courseId);

    Task<Faculty?> GetFacultyByEmail(string email);

    // ================= DATA =================

    Task<List<SubjectFaculty>> GetAllAsync();

    // ================= CRUD =================

    Task<(bool, string)> AddAsync(SubjectFaculty model);

    Task<(bool, string)> UpdateAsync(SubjectFaculty model); // 🔥 EDIT

    Task ToggleAsync(int id);

    // ================= CSV =================

    Task<(bool, string)> BulkInsertAsync(List<SubjectFaculty> list, int userId); // basic

    
}