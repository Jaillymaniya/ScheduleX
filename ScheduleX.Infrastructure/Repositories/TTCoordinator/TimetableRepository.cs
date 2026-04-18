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
                        var subjectFaculties = await _context.SubjectFaculties
                            .Where(x => x.DivisionId == div.DivisionId)
                            .ToListAsync();

                        // 🔥 workload only subject (NO faculty stored)
                        var workload = new List<int>();

                        foreach (var sub in subjects)
                        {
                            var lec = sub.LectureConfigs.FirstOrDefault();
                            var hasFaculty = subjectFaculties
                                .Any(f => f.SubjectSemesterId == sub.SubjectSemesterId);

                            if (lec == null || !hasFaculty) continue;

                            for (int i = 0; i < lec.TheoryLecturesPerWeek; i++)
                            {
                                workload.Add(sub.SubjectSemesterId);
                            }
                        }

                        int index = 0;

                        for (int day = 1; day <= 5; day++)
                        {
                            foreach (var slot in timeSlots)
                            {
                                if (index >= workload.Count) break;

                                var subjectSemesterId = workload[index];

                                // 🔥 prevent duplicate subject in same slot/division
                                bool clash = entries.Any(e =>
                                    e.DivisionId == div.DivisionId &&
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

                                    SubjectSemesterId = subjectSemesterId,
                                    RoomId = null
                                });

                                index++;
                            }
                        }
                    }
                }

                await _context.TimeTableEntries.AddRangeAsync(entries);
                await _context.SaveChangesAsync();

                // 🔥 IMPORTANT: load navigation for preview
                var result = await _context.TimeTableEntries
                    .Include(e => e.TimeSlot)
                    .Include(e => e.Division)
                    .Include(e => e.Room)
                    .Include(e => e.SubjectSemester)
                        .ThenInclude(ss => ss.Subject)
                    .Include(e => e.SubjectSemester)
                        .ThenInclude(ss => ss.SubjectFaculties)
                            .ThenInclude(sf => sf.Faculty)
                    .Where(e => e.BatchId == batch.BatchId)
                    .ToListAsync();

                return (true, "Timetable generated successfully", result);
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