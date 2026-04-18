using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Services.TimeTable
{
    public interface ITimeTableService
    {
        Task<GenerateResultDto> GenerateAsync(GenerateTTDto dto);
      
        Task<GenerateResultDto> GetPreviewByBatch(int batchId);
    }
}