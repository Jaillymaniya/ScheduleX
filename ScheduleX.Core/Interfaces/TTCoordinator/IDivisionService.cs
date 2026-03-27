

using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator;

public interface IDivisionService
{
    Task<List<Division>> GetAllDivisionsAsync();
    Task AddDivisionAsync(Division division);
    Task UpdateDivisionAsync(Division division);

    Task DeactivateDivisionAsync(int id);
    Task ActivateDivisionAsync(int id);

    Task CreateDivisionsAsync(int semesterId, int maxPerDivision);
}