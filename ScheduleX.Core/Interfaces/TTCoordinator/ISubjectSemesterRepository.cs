using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator;

public interface ISubjectSemesterRepository
{
    Task<List<Course>> GetCoursesAsync(int departmentId);
    Task<List<Semester>> GetSemestersByCourse(int courseId);
    Task<List<Subject>> GetSubjectsByCourse(int courseId);
    Task<List<Semester>> GetAllSemestersAsync();
    Task<List<Subject>> GetAllSubjectsAsync();
    Task<List<SubjectSemester>> GetAllAsync();

    Task<(bool, string)> AddAsync(SubjectSemester model);
    Task<(bool, string)> BulkInsertAsync(List<SubjectSemester> list);
    Task UpdateAsync(SubjectSemester model);
    Task SoftDeleteAsync(int id);
    Task ActivateAsync(int id);
}