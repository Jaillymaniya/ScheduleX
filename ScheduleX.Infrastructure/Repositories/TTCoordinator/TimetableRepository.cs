//using ScheduleX.Core.Entities;
//using ScheduleX.Core.Interfaces.TTCoordinator;
//using ScheduleX.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using static System.Reflection.Metadata.BlobBuilder;

//namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

//public class TimetableRepository(AppDbContext context) : ITimetableRepository
//{
//    private readonly AppDbContext _context = context;

//    // Moving this INSIDE the class or namespace clearly to fix "Type not found"
//    internal class SubjectWorkload
//    {
//        public int SubjectSemesterId { get; set; }
//        public int FacultyId { get; set; }
//        public int RemainingTheory { get; set; }
//        public int RemainingPracticalBlocks { get; set; }
//        public int BlockSize { get; set; }
//        public RoomTypeEnum PreferredRoomType { get; set; }
//    }

//    public async Task<(bool Success, string Message, List<TimeTableEntry> Entries)> GenerateAsync(
//    int userId, int courseId, List<int> semesterIds, int templateId)
//    {
//        try
//        {
//            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
//            var config = await _context.ScheduleConfigs
//                .Include(c => c.BreakRules)
//                .FirstOrDefaultAsync(x => x.CourseId == courseId && x.IsActive);

//            if (config == null) return (false, "Schedule configuration not found", null);

//            var timeSlots = await _context.TimeSlots
//                .Where(x => x.ConfigId == config.ConfigId)
//                .OrderBy(x => x.SlotNo).ToListAsync();

//            var globalFacultyTracker = new Dictionary<(int day, int slot), HashSet<int>>();
//            var globalRoomTracker = new Dictionary<(int day, int slot), HashSet<int>>();
//            var facultyDailyCount = new Dictionary<(int day, int facultyId), int>();
//            var allEntries = new List<TimeTableEntry>();

//            // 🔥 FIX 1: Initialize Batch with Semesters immediately to avoid FK Error 547
//            var batch = new TimeTableBatch
//            {
//                CreatedByUserId = userId,
//                DepartmentId = user?.DepartmentId ?? 0,
//                CourseId = courseId,
//                ConfigId = config.ConfigId,
//                TemplateId = templateId,
//                Status = BatchStatusEnum.Generated,
//                CreatedAt = DateTime.Now,
//                BatchSemesters = semesterIds.Select(id => new TimeTableBatchSemester { SemesterId = id }).ToList()
//            };

//            _context.TimeTableBatches.Add(batch);
//            // Save here so BatchId is generated for the entries below
//            await _context.SaveChangesAsync();

//            var workingDays = GetWorkingDays(config.WorkingDaysMask);

//            foreach (var semId in semesterIds)
//            {
//                var subjects = await _context.SubjectSemesters
//                    .Include(x => x.Subject)
//                    .Include(x => x.LectureConfigs)
//                    .Where(x => x.SemesterId == semId).ToListAsync();

//                var divisions = await _context.Divisions.Where(x => x.SemesterId == semId).ToListAsync();

//                foreach (var div in divisions)
//                {
//                    var workloads = await LoadWorkloads(subjects, div.DivisionId);
//                    var facultyAvail = await GetFacultyAvailability(workloads.Select(w => w.FacultyId).Distinct().ToList());

//                    foreach (var day in workingDays)
//                    {
//                        int lecturesPlacedToday = 0;

//                        for (int i = 0; i < timeSlots.Count; i++)
//                        {
//                            var slot = timeSlots[i];

//                            // 🔥 FIX 2: BREAK PROTECTION 
//                            // Using 'continue' ensures we don't fall through to the "Free" slot logic
//                            if (slot.SlotType == SlotTypeEnum.Break)
//                            {
//                                allEntries.Add(new TimeTableEntry
//                                {
//                                    BatchId = batch.BatchId,
//                                    SemesterId = semId,
//                                    DivisionId = div.DivisionId,
//                                    DayOfWeek = (byte)day,
//                                    TimeSlotId = slot.TimeSlotId,
//                                    EntryType = EntryTypeEnum.Break
//                                });
//                                continue;
//                            }

//                            // 🔥 FIX 3: IMPROVED SELECTION (Randomness fills Fridays)
//                            var selected = workloads
//                                .Where(w =>
//                                    (w.RemainingPracticalBlocks > 0 && CanFitBlock(w, i, timeSlots, day, globalFacultyTracker, facultyAvail, facultyDailyCount))
//                                    || (w.RemainingTheory > 0 && (!globalFacultyTracker.ContainsKey((day, slot.SlotNo)) || !globalFacultyTracker[(day, slot.SlotNo)].Contains(w.FacultyId)))
//                                )
//                                .OrderByDescending(w => w.RemainingPracticalBlocks > 0)
//                                .ThenBy(w => Guid.NewGuid()) // Randomness prevents the same subjects hogging Mon-Tue
//                                .FirstOrDefault();

//                            if (selected != null)
//                            {
//                                bool isPractical = selected.RemainingPracticalBlocks > 0;
//                                int blockSize = isPractical ? selected.BlockSize : 1;

//                                // 🔥 Lab Room logic is handled inside AllocateRoom
//                                var roomId = AllocateRoom(globalRoomTracker, div.DivisionId, selected, day, slot.SlotNo);

//                                Guid blockId = Guid.NewGuid();
//                                for (int b = 0; b < blockSize; b++)
//                                {
//                                    int currentIdx = i + b;
//                                    if (currentIdx >= timeSlots.Count) break;
//                                    var currentSlot = timeSlots[currentIdx];

//                                    allEntries.Add(new TimeTableEntry
//                                    {
//                                        BatchId = batch.BatchId,
//                                        SemesterId = semId,
//                                        DivisionId = div.DivisionId,
//                                        DayOfWeek = (byte)day,
//                                        TimeSlotId = currentSlot.TimeSlotId,
//                                        EntryType = EntryTypeEnum.Lecture,
//                                        SubjectSemesterId = selected.SubjectSemesterId,
//                                        RoomId = roomId,
//                                        BlockId = isPractical ? blockId : null
//                                    });

//                                    TrackCollision(globalFacultyTracker, day, currentSlot.SlotNo, selected.FacultyId);
//                                    if (roomId.HasValue) TrackCollision(globalRoomTracker, day, currentSlot.SlotNo, roomId.Value);

//                                    var fKey = (day, selected.FacultyId);
//                                    facultyDailyCount[fKey] = facultyDailyCount.GetValueOrDefault(fKey) + 1;
//                                }

//                                if (isPractical) { selected.RemainingPracticalBlocks--; i += (blockSize - 1); }
//                                else { selected.RemainingTheory--; }

//                                lecturesPlacedToday++;
//                            }
//                            else
//                            {
//                                allEntries.Add(CreateEntry(batch.BatchId, semId, div.DivisionId, day, slot.TimeSlotId, EntryTypeEnum.Free));
//                            }
//                        }
//                    }
//                }
//            }

//            await _context.TimeTableEntries.AddRangeAsync(allEntries);
//            await _context.SaveChangesAsync();

//            return (true, "Timetable generated successfully", allEntries);
//        }
//        catch (Exception ex)
//        {
//            return (false, ex.Message, null);
//        }
//    }

//    public async Task<List<Course>> GetCoursesForCoordinatorAsync(int userId) =>
//        await _context.TTCoordinatorCourses.Include(x => x.Course).Where(x => x.UserId == userId).Select(x => x.Course).ToListAsync();

//    public async Task<List<Semester>> GetSemestersByCourseAsync(int courseId) =>
//        await _context.Semesters.Where(x => x.CourseId == courseId).ToListAsync();

//    public async Task<List<TimeTableTemplate>> GetTemplatesAsync() =>
//        await _context.TimeTableTemplates.Where(x => x.IsActive).OrderByDescending(x => x.IsDefault).ToListAsync();

//    //public async Task<List<TimeTableBatch>> GetAllBatches() =>
//    //    await _context.TimeTableBatches.Include(x => x.Course).Include(x => x.BatchSemesters).ThenInclude(bs => bs.Semester).OrderByDescending(x => x.CreatedAt).ToListAsync();

//    public async Task<List<TimeTableBatch>> GetAllBatches()
//    {
//        return await _context.TimeTableBatches
//            .Include(x => x.Course)
//            .Include(x => x.BatchSemesters)
//               .ThenInclude(bs => bs.Semester)  // 🔥 THIS IS MISSING
//            .OrderByDescending(x => x.CreatedAt)
//            .ToListAsync();
//    }

//    public async Task<List<TimeTableEntry>> GetEntriesByBatch(int batchId) =>
//        await _context.TimeTableEntries.Where(x => x.BatchId == batchId).Include(x => x.TimeSlot).Include(x => x.Room).Include(x => x.Division).Include(x => x.SubjectSemester).ThenInclude(ss => ss.Subject).Include(x => x.SubjectSemester).ThenInclude(ss => ss.SubjectFaculties).ThenInclude(sf => sf.Faculty).ToListAsync();

//    private static void TrackCollision(Dictionary<(int day, int slot), HashSet<int>> tracker, int day, int slot, int id)
//    {
//        var key = (day, slot);
//        if (!tracker.ContainsKey(key)) tracker[key] = new HashSet<int>();
//        tracker[key].Add(id);
//    }

//    private static TimeTableEntry CreateEntry(int bId, int sId, int dId, int day, int slotId, EntryTypeEnum type)
//        => new() { BatchId = bId, SemesterId = sId, DivisionId = dId, DayOfWeek = (byte)day, TimeSlotId = slotId, EntryType = type };

//    private static List<int> GetWorkingDays(int mask)
//    {
//        var days = new List<int>();
//        for (int i = 0; i < 7; i++) if ((mask & (1 << i)) != 0) days.Add(i + 1);
//        return days;
//    }

//    private bool CanFitBlock(SubjectWorkload w, int currentIdx, List<TimeSlot> slots, int day,
//        Dictionary<(int day, int slot), HashSet<int>> fTrack, List<FacultyAvailability> avail, Dictionary<(int day, int facultyId), int> dailyCount)
//    {
//        var faculty = _context.Faculties.Find(w.FacultyId);
//        // Explicitly using the Key to avoid Dictionary inference issues
//        var countKey = (day: day, facultyId: w.FacultyId);
//        int currentDailyCount = dailyCount.ContainsKey(countKey) ? dailyCount[countKey] : 0;

//        if (faculty?.MaxLecturesPerDay > 0 && currentDailyCount >= faculty.MaxLecturesPerDay)
//            return false;

//        for (int j = 0; j < w.BlockSize; j++)
//        {
//            int nextIdx = currentIdx + j;
//            if (nextIdx >= slots.Count) return false;
//            var s = slots[nextIdx];
//            if (s.SlotType != SlotTypeEnum.Lecture) return false;
//            if (fTrack.ContainsKey((day, s.SlotNo)) && fTrack[(day, s.SlotNo)].Contains(w.FacultyId)) return false;

//            // Checking availability based on TimeOnly
//            if (!avail.Any(a => a.FacultyId == w.FacultyId && a.DayOfWeek == day && s.StartTime >= a.StartTime && s.StartTime < a.EndTime))
//                return false;
//        }
//        return true;
//    }

//    //private int? AllocateRoom(Dictionary<(int day, int slot), HashSet<int>> tracker, int divId, SubjectWorkload w, int day, int slot)
//    //{
//    //    var fixedRoom = _context.DivisionRoomAllocations.FirstOrDefault(r => r.DivisionId == divId);
//    //    if (fixedRoom != null && (!tracker.ContainsKey((day, slot)) || !tracker[(day, slot)].Contains(fixedRoom.RoomId)))
//    //        return fixedRoom.RoomId;

//    //    var rooms = _context.Rooms.Where(r => r.IsActive && r.RoomType == w.PreferredRoomType).ToList();
//    //    foreach (var r in rooms)
//    //        if (!tracker.ContainsKey((day, slot)) || !tracker[(day, slot)].Contains(r.RoomId)) return r.RoomId;

//    //    return null;
//    //}

//    private int? AllocateRoom(
//    Dictionary<(int day, int slot), HashSet<int>> tracker,
//    int divId,
//    SubjectWorkload w,
//    int day,
//    int slot)
//    {
//        // 🔥 1. Get required room type
//        var requiredType = w.PreferredRoomType;

//        // 🔥 2. Check fixed room BUT match type
//        var fixedRoom = _context.DivisionRoomAllocations
//            .Include(r => r.Room)
//            .FirstOrDefault(r => r.DivisionId == divId && r.Room.RoomType == requiredType);

//        if (fixedRoom != null)
//        {
//            if (!tracker.ContainsKey((day, slot)) ||
//                !tracker[(day, slot)].Contains(fixedRoom.RoomId))
//            {
//                return fixedRoom.RoomId;
//            }
//        }

//        // 🔥 3. Fallback → find any available room of required type
//        var rooms = _context.Rooms
//            .Where(r => r.IsActive && r.RoomType == requiredType)
//            .ToList();

//        foreach (var r in rooms)
//        {
//            if (!tracker.ContainsKey((day, slot)) ||
//                !tracker[(day, slot)].Contains(r.RoomId))
//            {
//                return r.RoomId;
//            }
//        }

//        // ❌ No room available
//        return null;
//    }

//    private async Task<List<SubjectWorkload>> LoadWorkloads(List<SubjectSemester> subjects, int divId)
//    {
//        var list = new List<SubjectWorkload>();
//        foreach (var s in subjects)
//        {
//            var lec = s.LectureConfigs.FirstOrDefault();
//            var sf = await _context.SubjectFaculties.FirstOrDefaultAsync(f => f.SubjectSemesterId == s.SubjectSemesterId && f.DivisionId == divId);
//            if (lec == null || sf == null) continue;

//            list.Add(new SubjectWorkload
//            {
//                SubjectSemesterId = s.SubjectSemesterId,
//                FacultyId = sf.FacultyId,
//                RemainingTheory = lec.TheoryLecturesPerWeek,
//                RemainingPracticalBlocks = (lec.PracticalBlockSize ?? 0) > 0 ? lec.PracticalLecturesPerWeek / lec.PracticalBlockSize.Value : 0,
//                BlockSize = lec.PracticalBlockSize ?? 1,
//                PreferredRoomType = s.Subject.SubjectCategory == SubjectCategoryEnum.Practical ? RoomTypeEnum.Lab : RoomTypeEnum.Classroom
//            });
//        }
//        return list;
//    }


//    public async Task<TimeTableBatch> GetBatchWithTemplate(int batchId)
//    {
//        return await _context.TimeTableBatches
//            .Include(x => x.TimeTableTemplate) //  IMPORTANT
//            .FirstOrDefaultAsync(x => x.BatchId == batchId);
//    }




//    private async Task<List<FacultyAvailability>> GetFacultyAvailability(List<int> facultyIds)
//        => await _context.FacultyAvailabilities.Where(a => facultyIds.Contains(a.FacultyId) && a.IsAvailable).ToListAsync();
//}




//for edit
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

public class TimetableRepository : ITimetableRepository
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public TimetableRepository(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    internal class SubjectWorkload
    {
        public int SubjectSemesterId { get; set; }
        public int FacultyId { get; set; }
        public int RemainingTheory { get; set; }
        public int RemainingPracticalBlocks { get; set; }
        public int BlockSize { get; set; }
        public RoomTypeEnum PreferredRoomType { get; set; }
    }

    // ================= GENERATE =================

    public async Task<(bool Success, string Message, List<TimeTableEntry> Entries)> GenerateAsync(
        int userId, int courseId, List<int> semesterIds, int templateId)
    {
        using var context = await _factory.CreateDbContextAsync();

        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            var config = await context.ScheduleConfigs
                .Include(c => c.BreakRules)
                .FirstOrDefaultAsync(x => x.CourseId == courseId && x.IsActive);

            if (config == null)
                return (false, "Schedule configuration not found", new List<TimeTableEntry>());

            var timeSlots = await context.TimeSlots
                .Where(x => x.ConfigId == config.ConfigId)
                .OrderBy(x => x.SlotNo)
                .ToListAsync();

            var allEntries = new List<TimeTableEntry>();

            var batch = new TimeTableBatch
            {
                CreatedByUserId = userId,
                DepartmentId = user?.DepartmentId ?? 0,
                CourseId = courseId,
                ConfigId = config.ConfigId,
                TemplateId = templateId,
                Status = BatchStatusEnum.Generated,
                CreatedAt = DateTime.Now,
                BatchSemesters = semesterIds.Select(id =>
                    new TimeTableBatchSemester { SemesterId = id }).ToList()
            };

            context.TimeTableBatches.Add(batch);
            await context.SaveChangesAsync();

            var workingDays = GetWorkingDays(config.WorkingDaysMask);

            foreach (var semId in semesterIds)
            {
                var divisions = await context.Divisions
                    .Where(x => x.SemesterId == semId)
                    .ToListAsync();

                foreach (var div in divisions)
                {
                    foreach (var day in workingDays)
                    {
                        foreach (var slot in timeSlots)
                        {
                            var type = slot.SlotType == SlotTypeEnum.Break
                                ? EntryTypeEnum.Break
                                : EntryTypeEnum.Free;

                            allEntries.Add(CreateEntry(batch.BatchId, semId, div.DivisionId, day, slot.TimeSlotId, type));
                        }
                    }
                }
            }

            await context.TimeTableEntries.AddRangeAsync(allEntries);
            await context.SaveChangesAsync();

            return (true, "Timetable generated successfully", allEntries);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, new List<TimeTableEntry>());
        }
    }

    // ================= FETCH =================

    public async Task<List<TimeTableBatch>> GetAllBatches()
    {
        using var context = await _factory.CreateDbContextAsync();

        return await context.TimeTableBatches
            .Include(x => x.Course)
            .Include(x => x.BatchSemesters)
                .ThenInclude(bs => bs.Semester)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TimeTableEntry>> GetEntriesByBatch(int batchId)
    {
        using var context = await _factory.CreateDbContextAsync();

        return await context.TimeTableEntries
            .Where(x => x.BatchId == batchId)
            .Include(x => x.TimeSlot)
            .Include(x => x.Room)
            .Include(x => x.Division)
            .Include(x => x.SubjectSemester).ThenInclude(s => s.Subject)
            .Include(x => x.SubjectSemester).ThenInclude(s => s.SubjectFaculties).ThenInclude(sf => sf.Faculty)
            .ToListAsync();
    }

    // ================= UPDATE =================

    public async Task<(bool Success, string Message)> UpdateEntryAsync(
        int entryId,
        int? subjectSemesterId,
        int? roomId,
        int userId)
    {
        using var context = await _factory.CreateDbContextAsync();

        var entry = await context.TimeTableEntries
            .FirstOrDefaultAsync(x => x.EntryId == entryId);

        if (entry == null)
            return (false, "Entry not found");

        if (entry.EntryType != EntryTypeEnum.Lecture)
            return (false, "Cannot edit this slot");

        entry.SubjectSemesterId = subjectSemesterId;
        entry.RoomId = roomId;

        await context.SaveChangesAsync();

        return (true, "Updated successfully");
    }

    // ================= OTHER =================

    public async Task<List<Course>> GetCoursesForCoordinatorAsync(int userId)
    {
        using var context = await _factory.CreateDbContextAsync();

        return await context.TTCoordinatorCourses
            .Include(x => x.Course)
            .Where(x => x.UserId == userId)
            .Select(x => x.Course)
            .ToListAsync();
    }

    public async Task<List<Semester>> GetSemestersByCourseAsync(int courseId)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.Semesters.Where(x => x.CourseId == courseId).ToListAsync();
    }

    public async Task<List<TimeTableTemplate>> GetTemplatesAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.TimeTableTemplates.Where(x => x.IsActive).ToListAsync();
    }

    public async Task<TimeTableBatch> GetBatchWithTemplate(int batchId)
    {
        using var context = await _factory.CreateDbContextAsync();

        return await context.TimeTableBatches
            .Include(x => x.TimeTableTemplate)
            .FirstOrDefaultAsync(x => x.BatchId == batchId);
    }

    // ================= HELPERS =================

    private static TimeTableEntry CreateEntry(int bId, int sId, int dId, int day, int slotId, EntryTypeEnum type)
        => new()
        {
            BatchId = bId,
            SemesterId = sId,
            DivisionId = dId,
            DayOfWeek = (byte)day,
            TimeSlotId = slotId,
            EntryType = type
        };

    private static List<int> GetWorkingDays(int mask)
    {
        var days = new List<int>();
        for (int i = 0; i < 7; i++)
            if ((mask & (1 << i)) != 0)
                days.Add(i + 1);
        return days;
    }

    //public async Task<(bool Success, string Message)> SwapEntriesAsync(int entryId1, int entryId2, int userId)
    //{
    //    using var context = await _factory.CreateDbContextAsync();

    //    var e1 = await context.TimeTableEntries.FindAsync(entryId1);
    //    var e2 = await context.TimeTableEntries.FindAsync(entryId2);

    //    if (e1 == null || e2 == null)
    //        return (false, "Entries not found");

    //    // ❌ prevent break swap
    //    if (e1.EntryType == EntryTypeEnum.Break || e2.EntryType == EntryTypeEnum.Break)
    //        return (false, "Cannot swap break slots");

    //    // 🔄 swap subject
    //    var tempSubject = e1.SubjectSemesterId;
    //    e1.SubjectSemesterId = e2.SubjectSemesterId;
    //    e2.SubjectSemesterId = tempSubject;

    //    // 🔄 swap room
    //    var tempRoom = e1.RoomId;
    //    e1.RoomId = e2.RoomId;
    //    e2.RoomId = tempRoom;

    //    // 🔄 swap entry type
    //    var tempType = e1.EntryType;
    //    e1.EntryType = e2.EntryType;
    //    e2.EntryType = tempType;

    //    await context.SaveChangesAsync();

    //    return (true, "Swapped successfully");
    //}


    public async Task<(bool Success, string Message)> SwapEntriesAsync(int entryId1, int entryId2, int userId)
    {
        using var context = await _factory.CreateDbContextAsync();

        var e1 = await context.TimeTableEntries
            .Include(x => x.SubjectSemester).ThenInclude(s => s.SubjectFaculties)
            .FirstOrDefaultAsync(x => x.EntryId == entryId1);

        var e2 = await context.TimeTableEntries
            .Include(x => x.SubjectSemester).ThenInclude(s => s.SubjectFaculties)
            .FirstOrDefaultAsync(x => x.EntryId == entryId2);

        if (e1 == null || e2 == null)
            return (false, "Entries not found");

        // ❌ BLOCK BREAK
        if (e1.EntryType == EntryTypeEnum.Break || e2.EntryType == EntryTypeEnum.Break)
            return (false, "Break not allowed");

        // ❌ SAME DIVISION ONLY
        if (e1.DivisionId != e2.DivisionId)
            return (false, "Only same division swap allowed");

        // =========================
        // 🔄 NEW VALUES
        // =========================
        var newSub1 = e2.SubjectSemesterId;
        var newSub2 = e1.SubjectSemesterId;

        // =========================
        // 🚨 FACULTY CHECK (GLOBAL)
        // =========================

        var faculty1 = e2.SubjectSemester?.SubjectFaculties?.FirstOrDefault()?.FacultyId;
        var faculty2 = e1.SubjectSemester?.SubjectFaculties?.FirstOrDefault()?.FacultyId;

        if (faculty1.HasValue)
        {
            var clash = await context.TimeTableEntries.AnyAsync(x =>
                x.EntryId != e1.EntryId &&
                x.DayOfWeek == e1.DayOfWeek &&
                x.TimeSlotId == e1.TimeSlotId &&
                x.SubjectSemester.SubjectFaculties.Any(f => f.FacultyId == faculty1)
            );

            if (clash)
                return (false, "Faculty clash");
        }

        if (faculty2.HasValue)
        {
            var clash = await context.TimeTableEntries.AnyAsync(x =>
                x.EntryId != e2.EntryId &&
                x.DayOfWeek == e2.DayOfWeek &&
                x.TimeSlotId == e2.TimeSlotId &&
                x.SubjectSemester.SubjectFaculties.Any(f => f.FacultyId == faculty2)
            );

            if (clash)
                return (false, "Faculty clash");
        }

        // =========================
        // 🔄 SWAP
        // =========================

        (e1.SubjectSemesterId, e2.SubjectSemesterId) =
            (e2.SubjectSemesterId, e1.SubjectSemesterId);

        e1.EntryType = e1.SubjectSemesterId.HasValue ? EntryTypeEnum.Lecture : EntryTypeEnum.Free;
        e2.EntryType = e2.SubjectSemesterId.HasValue ? EntryTypeEnum.Lecture : EntryTypeEnum.Free;

        await context.SaveChangesAsync();

        return (true, "Swapped");
    }



}


    private async Task<List<FacultyAvailability>> GetFacultyAvailability(List<int> facultyIds)
        => await _context.FacultyAvailabilities.Where(a => facultyIds.Contains(a.FacultyId) && a.IsAvailable).ToListAsync();


    public async Task<bool> DeleteBatchAsync(int batchId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1️⃣ Delete Entries
            var entries = await _context.TimeTableEntries
                .Where(x => x.BatchId == batchId)
                .ToListAsync();

            _context.TimeTableEntries.RemoveRange(entries);

            // 2️⃣ Delete Batch Semesters
            var semesters = await _context.TimeTableBatchSemesters
                .Where(x => x.BatchId == batchId)
                .ToListAsync();

            _context.TimeTableBatchSemesters.RemoveRange(semesters);

            // 3️⃣ Delete Batch
            var batch = await _context.TimeTableBatches
                .FirstOrDefaultAsync(x => x.BatchId == batchId);

            if (batch != null)
            {
                _context.TimeTableBatches.Remove(batch);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}