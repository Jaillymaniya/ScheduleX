using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator;

public interface IDivisionRoomAllocationRepository
{
    Task<List<DivisionRoomAllocation>> GetAllAsync();
    Task<DivisionRoomAllocation?> GetByIdAsync(int id);

    Task AddAsync(DivisionRoomAllocation entity);
    Task UpdateAsync(DivisionRoomAllocation entity);

    Task DeleteAsync(int id);

    Task<List<DivisionRoomAllocation>> GetBySemesterAsync(int semesterId);
}