using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

public class FacultyRepository : IFacultyRepository
{
    private readonly AppDbContext _context;

    public FacultyRepository(AppDbContext context)
    {
        _context = context;
    }

    // ===============================
    // ADD FACULTY
    // ===============================
    public async Task AddAsync(Faculty faculty)
    {
        faculty.IsActive = true; // ensure active by default
        faculty.CreatedAt = DateTime.Now;

        _context.Faculties.Add(faculty);
        await _context.SaveChangesAsync();
    }

    // ===============================
    // GET ALL FACULTY (Active + Inactive)
    // ===============================
    public async Task<List<Faculty>> GetAllAsync()
    {
        return await _context.Faculties
            .AsNoTracking()                 // 🔥 prevents UI stale render issue
            .Include(f => f.Department)     // 🔥 ensures Department loads
            .OrderByDescending(f => f.IsActive)
            .ThenBy(f => f.FacultyName)
            .ToListAsync();
    }


    // ===============================
    // GET DEPARTMENTS
    // ===============================
    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _context.Departments
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }

    // ===============================
    // UPDATE FACULTY (SAFE UPDATE)
    // ===============================
    public async Task UpdateAsync(Faculty faculty)
    {
        var existing = await _context.Faculties
            .FirstOrDefaultAsync(f => f.FacultyId == faculty.FacultyId);

        if (existing != null)
        {
            existing.FacultyName = faculty.FacultyName;
            existing.FacultyCode = faculty.FacultyCode;
            existing.Email = faculty.Email;
            existing.Phone = faculty.Phone;
            existing.DepartmentId = faculty.DepartmentId;
            existing.MaxLecturesPerDay = faculty.MaxLecturesPerDay;

            await _context.SaveChangesAsync();
        }
    }

    // ===============================
    // SOFT DELETE (Deactivate)
    // ===============================
    public async Task SoftDeleteAsync(int id)
    {
        var faculty = await _context.Faculties.FindAsync(id);

        if (faculty != null)
        {
            faculty.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    // ===============================
    // ACTIVATE FACULTY
    // ===============================
    public async Task ActivateAsync(int id)
    {
        var faculty = await _context.Faculties.FindAsync(id);

        if (faculty != null)
        {
            faculty.IsActive = true;
            await _context.SaveChangesAsync();
        }
    }
}