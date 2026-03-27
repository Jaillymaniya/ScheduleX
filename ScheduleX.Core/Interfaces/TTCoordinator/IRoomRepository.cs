

using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator;

public interface IRoomRepository
{
    Task AddRoomAsync(Room room);
    Task<List<Room>> GetAllRoomsAsync();
    Task<List<Department>> GetDepartmentsAsync();

    Task UpdateRoomAsync(Room room);

    Task DeleteRoomAsync(int id); // soft delete
    Task DeactivateRoomAsync(int id);
    Task ActivateRoomAsync(int id);
}