using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
    public interface ITimetableRepository
    {
        Task<(bool Success, string Message, List<TimeTableEntry> Entries)> GenerateAsync(
            int userId,
            int courseId,
            List<int> semesterIds,
            int templateId
        );

        Task<List<Course>> GetCoursesForCoordinatorAsync(int userId);
        Task<List<Semester>> GetSemestersByCourseAsync(int courseId);
        Task<List<TimeTableTemplate>> GetTemplatesAsync();
        Task<List<TimeTableBatch>> GetAllBatches();
       
        Task<List<TimeTableEntry>> GetEntriesByBatch(int batchId);

    }
}