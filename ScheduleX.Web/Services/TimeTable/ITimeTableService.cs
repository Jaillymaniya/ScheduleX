using ScheduleX.Core.Entities;
using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Services.TimeTable
{
    public interface ITimeTableService
    {
        Task<GenerateResultDto> GenerateAsync(GenerateTTDto dto);
      
        Task<GenerateResultDto> GetPreviewByBatch(int batchId);
        //Task<(bool Success, string Message)> UpdateEntryAsync(UpdateEntryDto dto);
        //Task<List<SubjectSemester>> GetSubjects();
        //Task<List<Room>> GetRooms();

        Task<(bool Success, string Message)> SwapEntriesAsync(SwapEntryDto dto);
    }
}