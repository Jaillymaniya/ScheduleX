

using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRoomAsync(Room room)
    {
        room.IsActive = true;
        room.CreatedAt = DateTime.Now;

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Room>> GetAllRoomsAsync()
    {
        return await _context.Rooms
            .Include(r => r.Department)
            .OrderByDescending(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }

    public async Task UpdateRoomAsync(Room room)
    {
        var existing = await _context.Rooms.FindAsync(room.RoomId);

        if (existing != null)
        {
            existing.RoomName = room.RoomName;
            existing.RoomType = room.RoomType;
            existing.Capacity = room.Capacity;
            existing.DepartmentId = room.DepartmentId;

            await _context.SaveChangesAsync();
        }
    }

    // ✅ SOFT DELETE
    public async Task DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room != null)
        {
            room.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeactivateRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room != null)
        {
            room.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ActivateRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room != null)
        {
            room.IsActive = true;
            await _context.SaveChangesAsync();
        }
    }
}