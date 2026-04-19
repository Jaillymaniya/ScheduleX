//using Microsoft.EntityFrameworkCore;
//using ScheduleX.Core.Entities;
//using ScheduleX.Core.Interfaces.TTCoordinator;
//using ScheduleX.Web.DTOs;
//using ScheduleX.Web.Models.Template;
//using ScheduleX.Web.Services.Excel;

//namespace ScheduleX.Web.Services.TimeTable
//{
//    public class TimeTableService : ITimeTableService
//    {
//        private readonly ITimetableRepository _repo;
//        private readonly IExcelService _excel;

//        public TimeTableService(ITimetableRepository repo, IExcelService excel)
//        {
//            _repo = repo;
//            _excel = excel;
//        }

//        //public async Task<GenerateResultDto> GetPreviewByBatch(int batchId)
//        //{
//        //    // Here, your data is stored in 'entries'
//        //    var entries = await _repo.GetEntriesByBatch(batchId);

//        //    // Change 'result.Entries' to 'entries' to fix the error
//        //    var preview = entries.Select(e => new PreviewDto
//        //    {
//        //        Day = e.DayOfWeek,
//        //        Slot = e.TimeSlot?.SlotNo ?? 0,

//        //        Subject = e.EntryType switch
//        //        {
//        //            EntryTypeEnum.Break => e.TimeSlot?.BreakRule?.BreakName ?? "Break",
//        //            EntryTypeEnum.Free => "Free",
//        //            _ => e.SubjectSemester?.Subject?.SubjectName ?? "N/A"
//        //        },

//        //        Faculty = e.EntryType == EntryTypeEnum.Lecture
//        //            ? (e.SubjectSemester?.SubjectFaculties?
//        //                .FirstOrDefault(f => f.DivisionId == e.DivisionId)?
//        //                .Faculty?.FacultyName ?? "N/A")
//        //            : "",

//        //        Room = e.Room?.RoomName ?? "N/A",
//        //        Division = e.Division?.DivisionName ?? "N/A"
//        //    }).ToList();

//        //    preview = preview
//        //        .OrderBy(x => x.Day)
//        //        .ThenBy(x => x.Slot)
//        //        .ToList();

//        //    var excel = _excel.GenerateExcel(preview);

//        //    return new GenerateResultDto
//        //    {
//        //        Success = true,
//        //        Preview = preview,
//        //        Base64 = Convert.ToBase64String(excel)
//        //    };
//        //}
//        public async Task<GenerateResultDto> GetPreviewByBatch(int batchId)
//        {
//            var entries = await _repo.GetEntriesByBatch(batchId);
//            var batch = await _repo.GetBatchWithTemplate(batchId);

//            TemplateStyle style;

//            try
//            {
//                if (!string.IsNullOrEmpty(batch?.TimeTableTemplate?.TemplateJson))
//                {
//                    style = System.Text.Json.JsonSerializer.Deserialize<TemplateStyle>(
//                        batch.TimeTableTemplate.TemplateJson
//                    );
//                }
//                else
//                {
//                    style = GetDefaultStyle();
//                }
//            }
//            catch
//            {
//                style = GetDefaultStyle();
//            }

//            var preview = entries.Select(e => new PreviewDto
//            {
//                Day = e.DayOfWeek,
//                Slot = e.TimeSlot?.SlotNo ?? 0,

//                Subject = e.EntryType switch
//                {
//                    EntryTypeEnum.Break => e.TimeSlot?.BreakRule?.BreakName ?? "Break",
//                    EntryTypeEnum.Free => "Free",
//                    _ => e.SubjectSemester?.Subject?.SubjectName ?? "N/A"
//                },

//                Faculty = e.EntryType == EntryTypeEnum.Lecture
//                    ? e.SubjectSemester?.SubjectFaculties?
//                        .FirstOrDefault(f => f.DivisionId == e.DivisionId)?
//                        .Faculty?.FacultyName ?? ""
//                    : "",

//                Room = e.Room?.RoomName ?? "",
//                Division = e.Division?.DivisionName ?? ""
//            })
//            .OrderBy(x => x.Day)
//            .ThenBy(x => x.Slot)
//            .ToList();

//            var excel = _excel.GenerateExcel(preview, style);

//            return new GenerateResultDto
//            {
//                Success = true,
//                Preview = preview,
//                Base64 = Convert.ToBase64String(excel),
//                TemplateStyle = style
//            };
//        }

//        private TemplateStyle GetDefaultStyle()
//        {
//            return new TemplateStyle
//            {
//                headerBg = "#1e293b",
//                headerText = "#ffffff",
//                bodyBg = "#ffffff",
//                bodyText = "#111827",
//                borderColor = "#cbd5e1",
//                cellPadding = "8px",
//                fontSize = "14px",
//                showRoom = true,
//                showFaculty = true,
//                titleAlign = "center"
//            };
//        }

//        public async Task<GenerateResultDto> GenerateAsync(GenerateTTDto dto)
//        {
//            try
//            {
//                // Here, the repository returns a tuple where the entries are inside 'result.Entries'
//                var result = await _repo.GenerateAsync(
//                    dto.UserId,
//                    dto.CourseId,
//                    dto.SemesterIds,
//                    dto.TemplateId
//                );

//                if (!result.Success || result.Entries == null)
//                {
//                    return new GenerateResultDto
//                    {
//                        Success = false,
//                        Message = result.Message
//                    };
//                }
//                var batch = await _repo.GetBatchWithTemplate(result.Entries.First().BatchId);
//                TemplateStyle style;

//                if (!string.IsNullOrEmpty(batch?.TimeTableTemplate?.TemplateJson))
//                {
//                    style = System.Text.Json.JsonSerializer.Deserialize<TemplateStyle>(
//                        batch.TimeTableTemplate.TemplateJson
//                    );
//                }
//                else
//                {
//                    style = new TemplateStyle();
//                }

//                var preview = result.Entries.Select(e => new PreviewDto
//                {
//                    Day = e.DayOfWeek,
//                    Slot = e.TimeSlot?.SlotNo ?? 0,

//                    Subject = e.EntryType switch
//                    {
//                        EntryTypeEnum.Break => e.TimeSlot?.BreakRule?.BreakName ?? "Break",
//                        EntryTypeEnum.Free => "Free",
//                        _ => e.SubjectSemester?.Subject?.SubjectName ?? "N/A"
//                    },

//                    Faculty = e.EntryType == EntryTypeEnum.Lecture
//                        ? (e.SubjectSemester?.SubjectFaculties?
//                            .FirstOrDefault(f => f.DivisionId == e.DivisionId)?
//                            .Faculty?.FacultyName ?? "N/A")
//                        : "",

//                    Room = e.Room?.RoomName ?? "N/A",
//                    Division = e.Division?.DivisionName ?? "N/A"
//                }).ToList();

//                var excel = _excel.GenerateExcel(preview, style);

//                return new GenerateResultDto
//                {
//                    Success = true,
//                    Message = "Generated Successfully",
//                    Base64 = Convert.ToBase64String(excel),
//                    Preview = preview,
//                    TemplateStyle = style
//                };
//            }
//            catch (Exception ex)
//            {
//                return new GenerateResultDto
//                {
//                    Success = false,
//                    Message = $"System Error: {ex.Message}"
//                };
//            }
//        }

//    }
//}


//for edit
using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Web.DTOs;
using ScheduleX.Web.Models.Template;
using ScheduleX.Web.Services.Excel;

namespace ScheduleX.Web.Services.TimeTable
{
    public class TimeTableService : ITimeTableService
    {
        private readonly ITimetableRepository _repo;
        private readonly IExcelService _excel;

        // ✅ ADD THIS
        private readonly IDbContextFactory<AppDbContext> _factory;

        // ✅ UPDATE CONSTRUCTOR
        public TimeTableService(
            ITimetableRepository repo,
            IExcelService excel,
            IDbContextFactory<AppDbContext> factory)
        {
            _repo = repo;
            _excel = excel;
            _factory = factory;
        }

        // ================= PREVIEW =================

        public async Task<GenerateResultDto> GetPreviewByBatch(int batchId)
        {
            var entries = await _repo.GetEntriesByBatch(batchId);
            var batch = await _repo.GetBatchWithTemplate(batchId);

            TemplateStyle style;

            try
            {
                if (!string.IsNullOrEmpty(batch?.TimeTableTemplate?.TemplateJson))
                {
                    style = System.Text.Json.JsonSerializer.Deserialize<TemplateStyle>(
                        batch.TimeTableTemplate.TemplateJson
                    );
                }
                else
                {
                    style = GetDefaultStyle();
                }
            }
            catch
            {
                style = GetDefaultStyle();
            }

            var preview = entries.Select(e => new PreviewDto
            {
                EntryId = e.EntryId, // ✅ IMPORTANT
                SubjectSemesterId = e.SubjectSemesterId,
                RoomId = e.RoomId,

                Day = e.DayOfWeek,
                Slot = e.TimeSlot?.SlotNo ?? 0,

                Subject = e.EntryType switch
                {
                    EntryTypeEnum.Break => e.TimeSlot?.BreakRule?.BreakName ?? "Break",
                    EntryTypeEnum.Free => "Free",
                    _ => e.SubjectSemester?.Subject?.SubjectName ?? "N/A"
                },

                Faculty = e.EntryType == EntryTypeEnum.Lecture
                    ? e.SubjectSemester?.SubjectFaculties?
                        .FirstOrDefault(f => f.DivisionId == e.DivisionId)?
                        .Faculty?.FacultyName ?? ""
                    : "",

                Room = e.Room?.RoomName ?? "",
                Division = e.Division?.DivisionName ?? ""
            })
            .OrderBy(x => x.Day)
            .ThenBy(x => x.Slot)
            .ToList();

            var excel = _excel.GenerateExcel(preview, style);

            return new GenerateResultDto
            {
                Success = true,
                Preview = preview,
                Base64 = Convert.ToBase64String(excel),
                TemplateStyle = style
            };
        }

        // ================= GENERATE =================

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

                var batch = await _repo.GetBatchWithTemplate(result.Entries.First().BatchId);

                var style = new TemplateStyle();

                if (!string.IsNullOrEmpty(batch?.TimeTableTemplate?.TemplateJson))
                {
                    style = System.Text.Json.JsonSerializer.Deserialize<TemplateStyle>(
                        batch.TimeTableTemplate.TemplateJson
                    );
                }

                var preview = result.Entries.Select(e => new PreviewDto
                {
                    EntryId = e.EntryId,
                    SubjectSemesterId = e.SubjectSemesterId,
                    RoomId = e.RoomId,

                    Day = e.DayOfWeek,
                    Slot = e.TimeSlot?.SlotNo ?? 0,

                    Subject = e.EntryType switch
                    {
                        EntryTypeEnum.Break => "Break",
                        EntryTypeEnum.Free => "Free",
                        _ => e.SubjectSemester?.Subject?.SubjectName ?? "N/A"
                    },

                    Faculty = e.SubjectSemester?.SubjectFaculties?
                        .FirstOrDefault()?.Faculty?.FacultyName ?? "",

                    Room = e.Room?.RoomName ?? "",
                    Division = e.Division?.DivisionName ?? ""
                }).ToList();

                var excel = _excel.GenerateExcel(preview, style);

                return new GenerateResultDto
                {
                    Success = true,
                    Message = "Generated Successfully",
                    Base64 = Convert.ToBase64String(excel),
                    Preview = preview,
                    TemplateStyle = style
                };
            }
            catch (Exception ex)
            {
                return new GenerateResultDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ================= EDIT =================

        

        // ================= NEW METHODS (YOU ASKED) =================

        public async Task<List<SubjectSemester>> GetSubjects()
        {
            using var context = await _factory.CreateDbContextAsync();

            return await context.SubjectSemesters
                .Include(x => x.Subject)
                .ToListAsync();
        }

        public async Task<List<Room>> GetRooms()
        {
            using var context = await _factory.CreateDbContextAsync();

            return await context.Rooms
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        // ================= DEFAULT STYLE =================

        private TemplateStyle GetDefaultStyle()
        {
            return new TemplateStyle
            {
                headerBg = "#1e293b",
                headerText = "#ffffff",
                bodyBg = "#ffffff",
                bodyText = "#111827",
                borderColor = "#cbd5e1",
                cellPadding = "8px",
                fontSize = "14px",
                showRoom = true,
                showFaculty = true,
                titleAlign = "center"
            };
        }

        public async Task<(bool Success, string Message)> SwapEntriesAsync(SwapEntryDto dto)
        {
            return await _repo.SwapEntriesAsync(
                dto.EntryId1,
                dto.EntryId2,
                dto.UserId
            );
        }
    }
}