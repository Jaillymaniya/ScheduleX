using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Web.DTOs;
using ScheduleX.Web.Services.Excel;

namespace ScheduleX.Web.Services.TimeTable
{
    public class TimeTableService : ITimeTableService
    {
        private readonly ITimetableRepository _repo;
        private readonly IExcelService _excel;

        public TimeTableService(ITimetableRepository repo, IExcelService excel)
        {
            _repo = repo;
            _excel = excel;
        }

        public async Task<GenerateResultDto> GenerateAsync(GenerateTTDto dto)
        {
            try
            {
                var result = await _repo.GenerateAsync(
                    dto.UserId,
                    dto.CourseId,
                    dto.SemesterIds,
                    dto.TemplateId
                );

                if (!result.Success || result.Entries == null)
                {
                    return new GenerateResultDto
                    {
                        Success = false,
                        Message = result.Message
                    };
                }

                var preview = result.Entries.Select(e => new PreviewDto
                {
                    Day = e.DayOfWeek,
                    Slot = e.TimeSlot?.SlotNo ?? 0,

                    Subject = e.SubjectSemester?.Subject?.SubjectName ?? "N/A",

                    // ✅ FIXED + COMMA ADDED
                    Faculty = e.SubjectSemester?
                        .SubjectFaculties
                        .FirstOrDefault(f => f.DivisionId == e.DivisionId)?
                        .Faculty?.FacultyName ?? "N/A",

                    Room = e.Room?.RoomName ?? "N/A",
                    Division = e.Division?.DivisionName ?? "N/A"
                }).ToList();

                var excel = _excel.GenerateExcel(preview);

                return new GenerateResultDto
                {
                    Success = true,
                    Message = "Generated Successfully",
                    Base64 = Convert.ToBase64String(excel),
                    Preview = preview
                };
            }
            catch (Exception ex)
            {
                return new GenerateResultDto
                {
                    Success = false,
                    Message = $"System Error: {ex.Message}"
                };
            }
        }
    }
}