using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly AppDbContext _context;

        public TimetableRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool, string, List<TimeTableEntry>)> GenerateAsync(
       int userId,
       int courseId,
       List<int> semesterIds,
       int templateId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
                if (user == null)
                    return (false, "User not found", null);

                if (user.DepartmentId == null)
                    return (false, "Department not assigned", null);

                var config = await _context.ScheduleConfigs
                    .FirstOrDefaultAsync(x => x.CourseId == courseId && x.IsActive);

                if (config == null)
                    return (false, "Schedule configuration not found", null);

                var timeSlots = await _context.TimeSlots
                    .Where(x => x.ConfigId == config.ConfigId)
                    .OrderBy(x => x.SlotNo)
                    .ToListAsync();

                if (!timeSlots.Any())
                    return (false, "Time slots not configured", null);

                var batch = new TimeTableBatch
                {
                    CreatedByUserId = userId,
                    DepartmentId = user.DepartmentId.Value,
                    CourseId = courseId,
                    ConfigId = config.ConfigId,
                    TemplateId = templateId,
                    Status = BatchStatusEnum.Generated
                };

                _context.TimeTableBatches.Add(batch);
                await _context.SaveChangesAsync();

                var entries = new List<TimeTableEntry>();

                foreach (var semId in semesterIds)
                {
                    var subjects = await _context.SubjectSemesters
                        .Include(x => x.Subject)
                        .Include(x => x.LectureConfigs)
                        .Where(x => x.SemesterId == semId)
                        .ToListAsync();

                    var divisions = await _context.Divisions
                        .Where(x => x.SemesterId == semId)
                        .ToListAsync();

                    foreach (var div in divisions)
                    {
                        var faculties = await _context.SubjectFaculties
                            .Where(x => x.DivisionId == div.DivisionId)
                            .ToListAsync();

                        var workload = new List<(int subId, int facultyId)>();

                        foreach (var sub in subjects)
                        {
                            var lec = sub.LectureConfigs.FirstOrDefault();
                            var fac = faculties.FirstOrDefault(f => f.SubjectSemesterId == sub.SubjectSemesterId);

                            if (lec == null || fac == null) continue;

                            for (int i = 0; i < lec.TheoryLecturesPerWeek; i++)
                            {
                                workload.Add((sub.SubjectSemesterId, fac.FacultyId));
                            }
                        }

                        int index = 0;

                        for (int day = 1; day <= 5; day++)
                        {
                            foreach (var slot in timeSlots)
                            {
                                if (index >= workload.Count) break;

                                var item = workload[index];

                                bool clash = entries.Any(e =>
                                    e.FacultyId == item.facultyId &&
                                    e.DayOfWeek == day &&
                                    e.TimeSlotId == slot.TimeSlotId);

                                if (clash) continue;

                                entries.Add(new TimeTableEntry
                                {
                                    BatchId = batch.BatchId,
                                    SemesterId = semId,
                                    DivisionId = div.DivisionId,
                                    DayOfWeek = (byte)day,
                                    TimeSlotId = slot.TimeSlotId,
                                    EntryType = EntryTypeEnum.Lecture,

                                    SubjectSemesterId = item.subId,
                                    FacultyId = item.facultyId,
                                    RoomId = null
                                });

                                index++;
                            }
                        }
                    }
                }

                await _context.TimeTableEntries.AddRangeAsync(entries);
                await _context.SaveChangesAsync();

                return (true, "Timetable generated successfully", entries);
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message, null);
            }
        }
        // ================= DROPDOWNS =================

        public async Task<List<Course>> GetCoursesForCoordinatorAsync(int userId)
        {
            return await _context.TTCoordinatorCourses
                .Include(x => x.Course)
                .Where(x => x.UserId == userId)
                .Select(x => x.Course)
                .ToListAsync();
        }

        public async Task<List<Semester>> GetSemestersByCourseAsync(int courseId)
        {
            return await _context.Semesters
                .Where(x => x.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<List<TimeTableTemplate>> GetTemplatesAsync()
        {
            return await _context.TimeTableTemplates
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.IsDefault)
                .ToListAsync();
        }
    }
}