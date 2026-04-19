using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;


using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;
using static System.Reflection.Metadata.BlobBuilder;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

public class TimetableRepository(AppDbContext context) : ITimetableRepository
{
    private readonly AppDbContext _context = context;

    // Moving this INSIDE the class or namespace clearly to fix "Type not found"
    internal class SubjectWorkload
    {
        public int SubjectSemesterId { get; set; }
        public int FacultyId { get; set; }
        public int RemainingTheory { get; set; }
        public int RemainingPracticalBlocks { get; set; }
        public int BlockSize { get; set; }
        public RoomTypeEnum PreferredRoomType { get; set; }
    }

    public async Task<(bool Success, string Message, List<TimeTableEntry> Entries)> GenerateAsync(
    int userId, int courseId, List<int> semesterIds, int templateId)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            var config = await _context.ScheduleConfigs
              .Include(c => c.BreakRules)
              .FirstOrDefaultAsync(x => x.CourseId == courseId && x.IsActive);

            if (config == null) return (false, "Schedule configuration not found", null);

            var timeSlots = await _context.TimeSlots
              .Where(x => x.ConfigId == config.ConfigId)
              .OrderBy(x => x.SlotNo).ToListAsync();

            var globalFacultyTracker = new Dictionary<(int day, int slot), HashSet<int>>();
            var globalRoomTracker = new Dictionary<(int day, int slot), HashSet<int>>();
            var facultyDailyCount = new Dictionary<(int day, int facultyId), int>();

            var allEntries = new List<TimeTableEntry>();

            var batch = new TimeTableBatch
            {
                CreatedByUserId = userId,
                DepartmentId = user?.DepartmentId ?? 0,
                CourseId = courseId,
                ConfigId = config.ConfigId,
                TemplateId = templateId,
                Status = BatchStatusEnum.Generated,
                CreatedAt = DateTime.Now
            };
            _context.TimeTableBatches.Add(batch);
            await _context.SaveChangesAsync();

            var workingDays = GetWorkingDays(config.WorkingDaysMask);

            foreach (var semId in semesterIds)
            {
                // Ensure we load ALL subjects and their associated faculties correctly
                var subjects = await _context.SubjectSemesters
          .Include(x => x.Subject)
          .Include(x => x.LectureConfigs)
          .Where(x => x.SemesterId == semId).ToListAsync();

                var divisions = await _context.Divisions.Where(x => x.SemesterId == semId).ToListAsync();

                foreach (var div in divisions)
                {
                    var workloads = await LoadWorkloads(subjects, div.DivisionId);
                    var facultyAvail = await GetFacultyAvailability(workloads.Select(w => w.FacultyId).Distinct().ToList());

                    foreach (var day in workingDays)
                    {
                        int lectureCounter = 0;

                        for (int i = 0; i < timeSlots.Count; i++)
                        {
                            var slot = timeSlots[i];

                            // 1. CHECK FOR BREAK FIRST
                            //                        var breakRule = config.BreakRules
                            //.FirstOrDefault(b => b.AfterLectureNo == lectureCounter);

                            if (slot.SlotType == SlotTypeEnum.Break)
                            {
                                lectureCounter++;
                                allEntries.Add(new TimeTableEntry
                                {
                                    BatchId = batch.BatchId,
                                    SemesterId = semId,
                                    DivisionId = div.DivisionId,
                                    DayOfWeek = (byte)day,
                                    TimeSlotId = slot.TimeSlotId, // This links to the name "Lunch Break"
                                    EntryType = EntryTypeEnum.Break,
                                    // Ensure Room and Subject are null for breaks
                                    SubjectSemesterId = null,
                                    RoomId = null
                                });
                                continue;
                            }

                            // 2. SELECT SUBJECT
                            // We shuffle the workloads list each time to prevent the same subject from hogging the start of the week
                            var selected = workloads
  .Where(w =>
    (w.RemainingTheory > 0 || w.RemainingPracticalBlocks > 0) &&
    CanFitBlock(w, i, timeSlots, day, globalFacultyTracker, facultyAvail, facultyDailyCount)
  )
  .OrderByDescending(w => w.RemainingTheory + w.RemainingPracticalBlocks)
  .FirstOrDefault();

                            if (selected != null)
                            {
                                bool isPractical = selected.RemainingPracticalBlocks > 0;
                                int blockSize = isPractical ? selected.BlockSize : 1;
                                var roomId = AllocateRoom(globalRoomTracker, div.DivisionId, selected, day, slot.SlotNo);

                                Guid blockId = Guid.NewGuid();
                                for (int b = 0; b < blockSize; b++)
                                {
                                    int currentIdx = i + b;
                                    if (currentIdx >= timeSlots.Count) break;

                                    var currentSlot = timeSlots[currentIdx];

                                    // ❌ STOP if break comes in between
                                    if (currentSlot.SlotType != SlotTypeEnum.Lecture)
                                        break;

                                    // ❌ STOP if break rule applies here
                                    //var innerBreakRule = config.BreakRules
                                    //    .FirstOrDefault(br => br.AfterLectureNo == lectureCounter + b);

                                    //if (innerBreakRule != null)
                                    //    break;


                                    allEntries.Add(new TimeTableEntry
                                    {
                                        BatchId = batch.BatchId,
                                        SemesterId = semId,
                                        DivisionId = div.DivisionId,
                                        DayOfWeek = (byte)day,
                                        TimeSlotId = currentSlot.TimeSlotId,
                                        EntryType = EntryTypeEnum.Lecture,
                                        SubjectSemesterId = selected.SubjectSemesterId,
                                        RoomId = roomId,
                                        BlockId = isPractical ? blockId : null
                                    });

                                    TrackCollision(globalFacultyTracker, day, currentSlot.SlotNo, selected.FacultyId);
                                    if (roomId.HasValue) TrackCollision(globalRoomTracker, day, currentSlot.SlotNo, roomId.Value);

                                    var fKey = (day, selected.FacultyId);
                                    facultyDailyCount[fKey] = facultyDailyCount.GetValueOrDefault(fKey) + 1;
                                }

                                if (isPractical) { selected.RemainingPracticalBlocks--; i += (blockSize - 1); }
                                else { selected.RemainingTheory--; }

                                lectureCounter++;
                            }
                            //else
                            //{
                            //    allEntries.Add(CreateEntry(batch.BatchId, semId, div.DivisionId, day, slot.TimeSlotId, EntryTypeEnum.Free));
                            //}
                            else
                            {
                                var fallback = workloads
                  .Where(w =>
                    (w.RemainingTheory > 0 || w.RemainingPracticalBlocks > 0) &&
                    CanFitBlock(w, i, timeSlots, day, globalFacultyTracker, facultyAvail, facultyDailyCount)
                  )
                  .OrderByDescending(w => w.RemainingTheory + w.RemainingPracticalBlocks)
                  .FirstOrDefault();

                                if (fallback != null)
                                {
                                    bool isPractical = fallback.RemainingPracticalBlocks > 0;

                                    allEntries.Add(new TimeTableEntry
                                    {
                                        BatchId = batch.BatchId,
                                        SemesterId = semId,
                                        DivisionId = div.DivisionId,
                                        DayOfWeek = (byte)day,
                                        TimeSlotId = slot.TimeSlotId,
                                        EntryType = EntryTypeEnum.Lecture,
                                        SubjectSemesterId = fallback.SubjectSemesterId,
                                        RoomId = AllocateRoom(globalRoomTracker, div.DivisionId, fallback, day, slot.SlotNo)
                                    });

                                    if (isPractical)
                                        fallback.RemainingPracticalBlocks--;
                                    else
                                        fallback.RemainingTheory--;
                                }
                            }
                        }
                    }
                }
            }

            await _context.TimeTableEntries.AddRangeAsync(allEntries);
            await _context.SaveChangesAsync();

            return (true, "Timetable generated successfully", allEntries);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<List<Course>> GetCoursesForCoordinatorAsync(int userId) =>
      await _context.TTCoordinatorCourses.Include(x => x.Course).Where(x => x.UserId == userId).Select(x => x.Course).ToListAsync();

    public async Task<List<Semester>> GetSemestersByCourseAsync(int courseId) =>
      await _context.Semesters.Where(x => x.CourseId == courseId).ToListAsync();

    public async Task<List<TimeTableTemplate>> GetTemplatesAsync() =>
      await _context.TimeTableTemplates.Where(x => x.IsActive).OrderByDescending(x => x.IsDefault).ToListAsync();

    public async Task<List<TimeTableBatch>> GetAllBatches() =>
      await _context.TimeTableBatches.Include(x => x.Course).Include(x => x.BatchSemesters).ThenInclude(bs => bs.Semester).OrderByDescending(x => x.CreatedAt).ToListAsync();

    public async Task<List<TimeTableEntry>> GetEntriesByBatch(int batchId) =>
      await _context.TimeTableEntries.Where(x => x.BatchId == batchId).Include(x => x.TimeSlot).Include(x => x.Room).Include(x => x.Division).Include(x => x.SubjectSemester).ThenInclude(ss => ss.Subject).Include(x => x.SubjectSemester).ThenInclude(ss => ss.SubjectFaculties).ThenInclude(sf => sf.Faculty).ToListAsync();

    private static void TrackCollision(Dictionary<(int day, int slot), HashSet<int>> tracker, int day, int slot, int id)
    {
        var key = (day, slot);
        if (!tracker.ContainsKey(key)) tracker[key] = new HashSet<int>();
        tracker[key].Add(id);
    }

    private static TimeTableEntry CreateEntry(int bId, int sId, int dId, int day, int slotId, EntryTypeEnum type)
      => new() { BatchId = bId, SemesterId = sId, DivisionId = dId, DayOfWeek = (byte)day, TimeSlotId = slotId, EntryType = type };

    private static List<int> GetWorkingDays(int mask)
    {
        var days = new List<int>();
        for (int i = 0; i < 7; i++) if ((mask & (1 << i)) != 0) days.Add(i + 1);
        return days;
    }

    private bool CanFitBlock(SubjectWorkload w, int currentIdx, List<TimeSlot> slots, int day,
      Dictionary<(int day, int slot), HashSet<int>> fTrack, List<FacultyAvailability> avail, Dictionary<(int day, int facultyId), int> dailyCount)
    {
        var faculty = _context.Faculties.Find(w.FacultyId);
        // Explicitly using the Key to avoid Dictionary inference issues
        var countKey = (day: day, facultyId: w.FacultyId);
        int currentDailyCount = dailyCount.ContainsKey(countKey) ? dailyCount[countKey] : 0;

        if (faculty?.MaxLecturesPerDay > 0 && currentDailyCount >= faculty.MaxLecturesPerDay)
            return false;

        for (int j = 0; j < w.BlockSize; j++)
        {
            int nextIdx = currentIdx + j;
            if (nextIdx >= slots.Count) return false;
            var s = slots[nextIdx];
            if (s.SlotType != SlotTypeEnum.Lecture) return false;
            if (fTrack.ContainsKey((day, s.SlotNo)) && fTrack[(day, s.SlotNo)].Contains(w.FacultyId)) return false;

            // Checking availability based on TimeOnly
            if (!avail.Any(a => a.FacultyId == w.FacultyId && a.DayOfWeek == day && s.StartTime >= a.StartTime && s.StartTime < a.EndTime))
                return false;
        }
        return true;
    }

    //private int? AllocateRoom(Dictionary<(int day, int slot), HashSet<int>> tracker, int divId, SubjectWorkload w, int day, int slot)
    //{
    //    var fixedRoom = _context.DivisionRoomAllocations.FirstOrDefault(r => r.DivisionId == divId);
    //    if (fixedRoom != null && (!tracker.ContainsKey((day, slot)) || !tracker[(day, slot)].Contains(fixedRoom.RoomId)))
    //        return fixedRoom.RoomId;

    //    var rooms = _context.Rooms.Where(r => r.IsActive && r.RoomType == w.PreferredRoomType).ToList();
    //    foreach (var r in rooms)
    //        if (!tracker.ContainsKey((day, slot)) || !tracker[(day, slot)].Contains(r.RoomId)) return r.RoomId;

    //    return null;
    //}

    private int? AllocateRoom(
  Dictionary<(int day, int slot), HashSet<int>> tracker,
  int divId,
  SubjectWorkload w,
  int day,
  int slot)
    {
        // 🔥 1. Get required room type
        var requiredType = w.PreferredRoomType;

        // 🔥 2. Check fixed room BUT match type
        var fixedRoom = _context.DivisionRoomAllocations
      .Include(r => r.Room)
      .FirstOrDefault(r => r.DivisionId == divId && r.Room.RoomType == requiredType);

        if (fixedRoom != null)
        {
            if (!tracker.ContainsKey((day, slot)) ||
              !tracker[(day, slot)].Contains(fixedRoom.RoomId))
            {
                return fixedRoom.RoomId;
            }
        }

        // 🔥 3. Fallback → find any available room of required type
        var rooms = _context.Rooms
      .Where(r => r.IsActive && r.RoomType == requiredType)
      .ToList();

        foreach (var r in rooms)
        {
            if (r.RoomType == requiredType &&
              (!tracker.ContainsKey((day, slot)) ||
              !tracker[(day, slot)].Contains(r.RoomId)))
            {
                return r.RoomId;
            }
        }

        // ❌ No room available
        //return null;
        var room = _context.Rooms
  .FirstOrDefault(r => r.IsActive && r.RoomType == requiredType);

        if (room != null)
            return room.RoomId;

        // fallback ONLY if absolutely needed
        return _context.Rooms.FirstOrDefault()?.RoomId ?? 0;
    }

    private async Task<List<SubjectWorkload>> LoadWorkloads(List<SubjectSemester> subjects, int divId)
    {
        var list = new List<SubjectWorkload>();
        foreach (var s in subjects)
        {
            var lec = s.LectureConfigs.FirstOrDefault();
            var sf = await _context.SubjectFaculties.FirstOrDefaultAsync(f => f.SubjectSemesterId == s.SubjectSemesterId && f.DivisionId == divId);
            if (lec == null || sf == null) continue;

            list.Add(new SubjectWorkload
            {
                SubjectSemesterId = s.SubjectSemesterId,
                FacultyId = sf.FacultyId,
                RemainingTheory = lec.TheoryLecturesPerWeek,
                RemainingPracticalBlocks = (lec.PracticalBlockSize ?? 0) > 0 ? lec.PracticalLecturesPerWeek / lec.PracticalBlockSize.Value : 0,
                BlockSize = lec.PracticalBlockSize ?? 1,
                PreferredRoomType = s.Subject.SubjectCategory == SubjectCategoryEnum.Practical ? RoomTypeEnum.Lab : RoomTypeEnum.Classroom
            });
        }
        return list;
    }

    private async Task<List<FacultyAvailability>> GetFacultyAvailability(List<int> facultyIds)
      => await _context.FacultyAvailabilities.Where(a => facultyIds.Contains(a.FacultyId) && a.IsAvailable).ToListAsync();
}
