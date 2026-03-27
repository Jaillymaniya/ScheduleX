using ScheduleX.Core.Entities;

public interface ISubjectFacultyRepository
{
    Task<List<Course>> GetCoursesAsync();

    Task<List<Semester>> GetSemesters(int courseId);

    Task<List<Division>> GetDivisions(int semesterId);

    Task<List<SubjectSemester>> GetSubjectSemesters(int semesterId);

    Task<List<Faculty>> GetFaculties(int courseId);
    Task<List<Course>> GetCoursesByCoordinator(int userId);
    // 🔥 NEW (for other department)
    Task<List<Department>> GetDepartments();
    Task<List<Course>> GetCoursesByDepartment(int deptId);
    Task<List<Faculty>> GetFacultyByCourse(int courseId);
    Task<Faculty?> GetFacultyByEmail(string email);
    Task<List<SubjectFaculty>> GetAllAsync();

    Task<(bool, string)> AddAsync(SubjectFaculty model);

    Task ToggleAsync(int id);
}