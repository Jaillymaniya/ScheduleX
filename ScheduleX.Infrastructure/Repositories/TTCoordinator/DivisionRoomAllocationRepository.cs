using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories;

public class DivisionRoomAllocationRepository : IDivisionRoomAllocationRepository
{
    private readonly AppDbContext _context;

    public DivisionRoomAllocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DivisionRoomAllocation>> GetAllAsync()
    {
        return await _context.DivisionRoomAllocations
            .Include(x => x.Semester)
            .Include(x => x.Division)
            .Include(x => x.Room)
            .ToListAsync();
    }

    public async Task<DivisionRoomAllocation?> GetByIdAsync(int id)
    {
        return await _context.DivisionRoomAllocations
            .Include(x => x.Semester)
            .Include(x => x.Division)
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x => x.AllocationId == id);
    }

    //public async Task AddAsync(DivisionRoomAllocation entity)
    //{
    //    _context.DivisionRoomAllocations.Add(entity);
    //    await _context.SaveChangesAsync();
    //}

    //public async Task AddAsync(DivisionRoomAllocation entity)
    //{
    //    var exists = await _context.DivisionRoomAllocations
    //        .AnyAsync(x => x.RoomId == entity.RoomId);

    //    if (exists)
    //        throw new Exception("Room already assigned!");

    //    _context.DivisionRoomAllocations.Add(entity);
    //    await _context.SaveChangesAsync();
    //}


    public async Task AddAsync(DivisionRoomAllocation entity)
    {
        // ❌ Room duplicate
        var roomExists = await _context.DivisionRoomAllocations
            .AnyAsync(x => x.RoomId == entity.RoomId);

        if (roomExists)
            throw new Exception("Room is already allocated!");

        // ❌ Semester + Division duplicate
        var divisionExists = await _context.DivisionRoomAllocations
            .AnyAsync(x => x.SemesterId == entity.SemesterId
                        && x.DivisionId == entity.DivisionId);

        if (divisionExists)
            throw new Exception("This division is already assigned in this semester!");

        _context.DivisionRoomAllocations.Add(entity);
        await _context.SaveChangesAsync();
    }



    //public async Task UpdateAsync(DivisionRoomAllocation entity)
    //{
    //    _context.DivisionRoomAllocations.Update(entity);
    //    await _context.SaveChangesAsync();
    //}

    //public async Task UpdateAsync(DivisionRoomAllocation entity)
    //{
    //    var existing = await _context.DivisionRoomAllocations
    //        .FirstOrDefaultAsync(x => x.AllocationId == entity.AllocationId);

    //    if (existing == null)
    //        throw new Exception("Record not found");

    //    var exists = await _context.DivisionRoomAllocations
    //        .AnyAsync(x => x.RoomId == entity.RoomId && x.AllocationId != entity.AllocationId);

    //    if (exists)
    //        throw new Exception("Room is already allocated!");

    //    existing.SemesterId = entity.SemesterId;
    //    existing.DivisionId = entity.DivisionId;
    //    existing.RoomId = entity.RoomId;
    //    existing.IsFixed = entity.IsFixed;

    //    await _context.SaveChangesAsync();
    //}


    public async Task UpdateAsync(DivisionRoomAllocation entity)
    {
        var existing = await _context.DivisionRoomAllocations
            .FirstOrDefaultAsync(x => x.AllocationId == entity.AllocationId);

        if (existing == null)
            throw new Exception("Record not found");

        // ❌ Room duplicate check
        var roomExists = await _context.DivisionRoomAllocations
            .AnyAsync(x => x.RoomId == entity.RoomId
                        && x.AllocationId != entity.AllocationId);

        if (roomExists)
            throw new Exception("Room is already allocated!");

        // ❌ Semester + Division duplicate check
        var divisionExists = await _context.DivisionRoomAllocations
            .AnyAsync(x => x.SemesterId == entity.SemesterId
                        && x.DivisionId == entity.DivisionId
                        && x.AllocationId != entity.AllocationId);

        if (divisionExists)
            throw new Exception("This division is already assigned in this semester!");

        // ✅ Update
        existing.SemesterId = entity.SemesterId;
        existing.DivisionId = entity.DivisionId;
        existing.RoomId = entity.RoomId;
        existing.IsFixed = entity.IsFixed;

        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var data = await _context.DivisionRoomAllocations.FindAsync(id);
        if (data != null)
        {
            _context.DivisionRoomAllocations.Remove(data);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<DivisionRoomAllocation>> GetBySemesterAsync(int semesterId)
    {
        return await _context.DivisionRoomAllocations
            .Where(x => x.SemesterId == semesterId)
            .Include(x => x.Division)
            .Include(x => x.Room)
            .ToListAsync();
    }
}