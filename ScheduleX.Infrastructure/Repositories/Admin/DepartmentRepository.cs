using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAllAsync()
        => await _context.Departments.ToListAsync();

    public async Task AddAsync(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
    }

    public async Task ToggleStatusAsync(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept != null)
        {
            dept.IsActive = !dept.IsActive;
            await _context.SaveChangesAsync();
        }
    }
}