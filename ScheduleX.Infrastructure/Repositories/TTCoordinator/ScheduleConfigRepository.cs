using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

public class ScheduleConfigRepository : IScheduleConfigRepository
{
    private readonly AppDbContext _context;

    public ScheduleConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleConfig?> GetByCourseAsync(int courseId)
    {
        return await _context.ScheduleConfigs
            .Include(x => x.BreakRules.OrderBy(b => b.BreakNo))
            .FirstOrDefaultAsync(x => x.CourseId == courseId && x.IsActive);
    }

    public async Task<ScheduleConfig?> GetByConfigIdAsync(int configId)
    {
        return await _context.ScheduleConfigs
            .Include(x => x.BreakRules.OrderBy(b => b.BreakNo))
            .Include(x => x.TimeSlots.OrderBy(t => t.SlotNo))
            .FirstOrDefaultAsync(x => x.ConfigId == configId);
    }

    public async Task<ScheduleConfig> SaveAsync(ScheduleConfig config)
    {
        var existing = await _context.ScheduleConfigs
            .FirstOrDefaultAsync(x => x.CourseId == config.CourseId && x.IsActive);

        if (existing == null)
        {
            _context.ScheduleConfigs.Add(config);
        }
        else
        {
            existing.StartTime = config.StartTime;
            existing.EndTime = config.EndTime;
            existing.LectureDurationMin = config.LectureDurationMin;
            existing.WorkingDaysMask = config.WorkingDaysMask;
            existing.LecturesPerDay = config.LecturesPerDay;
            existing.IsActive = config.IsActive;
            config = existing;
        }

        await _context.SaveChangesAsync();
        return config;
    }

    public async Task<List<BreakRule>> GetBreakRulesAsync(int configId)
    {
        return await _context.BreakRules
            .Where(x => x.ConfigId == configId)
            .OrderBy(x => x.BreakNo)
            .ToListAsync();
    }

    public async Task<BreakRule> AddBreakRuleAsync(BreakRule breakRule)
    {
        _context.BreakRules.Add(breakRule);
        await _context.SaveChangesAsync();
        return breakRule;
    }

    public async Task<BreakRule?> UpdateBreakRuleAsync(BreakRule breakRule)
    {
        var existing = await _context.BreakRules.FirstOrDefaultAsync(x => x.BreakRuleId == breakRule.BreakRuleId);
        if (existing == null) return null;

        existing.BreakNo = breakRule.BreakNo;
        existing.AfterLectureNo = breakRule.AfterLectureNo;
        existing.BreakDurationMin = breakRule.BreakDurationMin;
        existing.BreakName = breakRule.BreakName;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteBreakRuleAsync(int breakRuleId)
    {
        var existing = await _context.BreakRules.FirstOrDefaultAsync(x => x.BreakRuleId == breakRuleId);
        if (existing == null) return false;

        _context.BreakRules.Remove(existing);
        await _context.SaveChangesAsync();
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<TimeSlot>> GetTimeSlotsAsync(int configId)
    {
        return await _context.TimeSlots
            .Include(x => x.BreakRule)
            .Where(x => x.ConfigId == configId)
            .OrderBy(x => x.SlotNo)
            .ToListAsync();
    }

    public async Task<List<TimeSlot>> GenerateTimeSlotsAsync(int configId)
    {
        var config = await _context.ScheduleConfigs
            .Include(x => x.BreakRules)
            .FirstOrDefaultAsync(x => x.ConfigId == configId);

        if (config == null)
            throw new Exception("Schedule config not found.");

        var breaks = config.BreakRules.OrderBy(x => x.AfterLectureNo).ThenBy(x => x.BreakNo).ToList();

        var totalLectureMinutes = config.LecturesPerDay * config.LectureDurationMin;
        var totalBreakMinutes = breaks.Sum(x => x.BreakDurationMin);
        var availableMinutes = (int)(config.EndTime.ToTimeSpan() - config.StartTime.ToTimeSpan()).TotalMinutes;

        if (totalLectureMinutes + totalBreakMinutes > availableMinutes)
            throw new Exception("Config time range is not enough for lectures and breaks.");

        var oldSlots = _context.TimeSlots.Where(x => x.ConfigId == configId);
        _context.TimeSlots.RemoveRange(oldSlots);
        await _context.SaveChangesAsync();

        var slots = new List<TimeSlot>();
        var currentTime = config.StartTime;
        byte slotNo = 1;

        for (byte lectureNo = 1; lectureNo <= config.LecturesPerDay; lectureNo++)
        {
            var lectureEnd = currentTime.AddMinutes(config.LectureDurationMin);

            slots.Add(new TimeSlot
            {
                ConfigId = configId,
                SlotNo = slotNo++,
                StartTime = currentTime,
                EndTime = lectureEnd,
                SlotType = SlotTypeEnum.Lecture,
                BreakRuleId = null
            });

            currentTime = lectureEnd;

            var breaksAfterThisLecture = breaks
                .Where(x => x.AfterLectureNo == lectureNo)
                .OrderBy(x => x.BreakNo)
                .ToList();

            foreach (var br in breaksAfterThisLecture)
            {
                var breakEnd = currentTime.AddMinutes(br.BreakDurationMin);

                slots.Add(new TimeSlot
                {
                    ConfigId = configId,
                    SlotNo = slotNo++,
                    StartTime = currentTime,
                    EndTime = breakEnd,
                    SlotType = SlotTypeEnum.Break,
                    BreakRuleId = br.BreakRuleId
                });

                currentTime = breakEnd;
            }
        }

        _context.TimeSlots.AddRange(slots);
        await _context.SaveChangesAsync();

        return await GetTimeSlotsAsync(configId);
    }

    public async Task<List<TimeTableTemplate>> GetActiveTemplatesAsync()
    {
        return await _context.TimeTableTemplates
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.TemplateName)
            .ToListAsync();
    }
}