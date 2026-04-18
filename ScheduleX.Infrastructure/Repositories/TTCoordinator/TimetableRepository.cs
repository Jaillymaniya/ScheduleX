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
            var subjectSem = await _context.SubjectSemesters
                .Where(x => semesterIds.Contains(x.SemesterId))
                .Include(x => x.Subject)
                .ToListAsync();

            if (!subjectSem.Any())
                return (false, "No subjects mapped", null);

            var lectureConfigs = await _context.SubjectLectureConfigs.ToListAsync();
            var facultyMap = await _context.SubjectFaculties.Include(x => x.Faculty).ToListAsync();
            var rooms = await _context.Rooms.Where(x => x.IsActive).ToListAsync();

            var batch = new TimeTableBatch
            {
                CreatedByUserId = userId,
                CourseId = courseId,
                TemplateId = templateId,
                Status = BatchStatusEnum.Generated
            };

            _context.TimeTableBatches.Add(batch);
            await _context.SaveChangesAsync();

            var entries = new List<TimeTableEntry>();

            foreach (var semId in semesterIds)
            {
                var divisions = await _context.Divisions
                    .Where(x => x.SemesterId == semId)
                    .ToListAsync();

                foreach (var div in divisions)
                {
                    for (int day = 1; day <= 5; day++)
                    {
                        for (int slot = 1; slot <= 6; slot++)
                        {
                            entries.Add(new TimeTableEntry
                            {
                                BatchId = batch.BatchId,
                                SemesterId = semId,
                                DivisionId = div.DivisionId,
                                DayOfWeek = (byte)day,
                                TimeSlotId = slot,
                                EntryType = EntryTypeEnum.Lecture,
                                RoomId = rooms.FirstOrDefault()?.RoomId
                            });
                        }
                    }
                }
            }

            await _context.TimeTableEntries.AddRangeAsync(entries);
            await _context.SaveChangesAsync();

            return (true, "Generated successfully", entries);
        }

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
            return await _context.TimeTableTemplates.ToListAsync();
        }
    }
}