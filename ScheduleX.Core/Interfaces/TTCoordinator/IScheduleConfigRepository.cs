using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator;

public interface IScheduleConfigRepository
{
    Task<ScheduleConfig?> GetByCourseAsync(int courseId);
    Task<ScheduleConfig?> GetByConfigIdAsync(int configId);
    Task<ScheduleConfig> SaveAsync(ScheduleConfig config);

    Task<List<BreakRule>> GetBreakRulesAsync(int configId);
    Task<BreakRule> AddBreakRuleAsync(BreakRule breakRule);
    Task<BreakRule?> UpdateBreakRuleAsync(BreakRule breakRule);
    Task<bool> DeleteBreakRuleAsync(int breakRuleId);

    Task<List<TimeSlot>> GenerateTimeSlotsAsync(int configId);
    Task<List<TimeSlot>> GetTimeSlotsAsync(int configId);

    Task<List<TimeTableTemplate>> GetActiveTemplatesAsync();
}
