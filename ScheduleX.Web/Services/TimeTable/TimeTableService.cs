using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Web.DTOs;
using ScheduleX.Web.Services.Excel;
using ScheduleX.Web.Services.TimeTable;

namespace ScheduleX.Web.Services.TimeTable
{

    public class TimeTableService : ITimeTableService
    {
        private readonly AppDbContext _context;
        private readonly IExcelService _excel;

        public TimeTableService(AppDbContext context, IExcelService excel)
        {
            _context = context;
            _excel = excel;
        }

        public async Task<GenerateResultDto> GenerateAsync(GenerateTTDto dto)
        {
            var subjectSem = _context.SubjectSemesters
                .Where(x => dto.SemesterIds.Contains(x.SemesterId))
                .ToList();

            if (!subjectSem.Any())
                return Fail("No subjects mapped");

            var lectureConfigs = _context.SubjectLectureConfigs.ToList();
            var facultyMap = _context.SubjectFaculties.ToList();
            var rooms = _context.Rooms.Where(x => x.IsActive).ToList();
            var availability = _context.FacultyAvailabilities.ToList();

            var batch = new TimeTableBatch
            {
                CreatedByUserId = dto.UserId,
                CourseId = dto.CourseId,
                TemplateId = dto.TemplateId,
                Status = BatchStatusEnum.Generated
            };

            _context.TimeTableBatches.Add(batch);
            await _context.SaveChangesAsync();

            var entries = new List<TimeTableEntry>();
            var preview = new List<PreviewDto>();

            var facultyBusy = new HashSet<string>();
            var roomBusy = new HashSet<string>();

            foreach (var semId in dto.SemesterIds)
            {
                var divisions = _context.Divisions.Where(x => x.SemesterId == semId).ToList();

                foreach (var div in divisions)
                {
                    for (int day = 1; day <= 5; day++)
                    {
                        facultyBusy.Clear();
                        roomBusy.Clear();

                        for (int slot = 1; slot <= 6; slot++)
                        {
                            var faculty = facultyMap.FirstOrDefault();

                            var room = rooms.FirstOrDefault(r =>
                                !roomBusy.Contains($"{day}-{slot}-{r.RoomId}"));

                            if (faculty != null && room != null)
                            {
                                entries.Add(new TimeTableEntry
                                {
                                    BatchId = batch.BatchId,
                                    SemesterId = semId,
                                    DivisionId = div.DivisionId,
                                    DayOfWeek = (byte)day,
                                    TimeSlotId = slot,
                                    EntryType = EntryTypeEnum.Lecture,
                                    RoomId = room.RoomId
                                });

                                preview.Add(new PreviewDto
                                {
                                    Day = day,
                                    Slot = slot,
                                    Subject = "Subject",
                                    Faculty = faculty.FacultyId.ToString(),
                                    Room = room.RoomName,
                                    Division = div.DivisionName
                                });
                            }
                        }
                    }
                }
            }

            await _context.TimeTableEntries.AddRangeAsync(entries);
            await _context.SaveChangesAsync();

            var excel = _excel.GenerateExcel(preview);

            return new GenerateResultDto
            {
                Success = true,
                Message = "Generated",
                Base64 = Convert.ToBase64String(excel),
                Preview = preview
            };
        }

        private GenerateResultDto Fail(string msg)
        {
            return new GenerateResultDto { Success = false, Message = msg };
        }
    }
}