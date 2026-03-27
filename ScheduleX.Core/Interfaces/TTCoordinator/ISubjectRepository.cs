using ScheduleX.Core.Entities;

public interface ISubjectRepository
{
    Task<List<Subject>> GetAllAsync();
    Task<List<Course>> GetCoursesAsync();

    Task<(bool, string)> AddAsync(Subject subject);
    Task<(bool, string)> UpdateAsync(Subject subject);

    Task SoftDeleteAsync(int id);
    Task ActivateAsync(int id);

    Task<bool> IsSubjectCodeExists(string code, int? id = null);

    Task<(bool, string)> BulkInsertAsync(List<Subject> subjects);
}