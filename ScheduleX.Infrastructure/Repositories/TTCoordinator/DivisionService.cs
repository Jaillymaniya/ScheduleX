


using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator;

public class DivisionService : IDivisionService
{
    private readonly AppDbContext _context;

    public DivisionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Division>> GetAllDivisionsAsync()
    {
        return await _context.Divisions
            .Include(d => d.Semester)
            .OrderByDescending(d => d.IsActive)
            .ToListAsync();
    }

    public async Task AddDivisionAsync(Division division)
    {
        division.IsActive = true;
        division.CreatedAt = DateTime.Now;

        _context.Divisions.Add(division);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDivisionAsync(Division division)
    {
        var existing = await _context.Divisions.FindAsync(division.DivisionId);

        if (existing != null)
        {
            existing.DivisionName = division.DivisionName;
            existing.StudentStrength = division.StudentStrength;
            existing.SemesterId = division.SemesterId;

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeactivateDivisionAsync(int id)
    {
        var d = await _context.Divisions.FindAsync(id);

        if (d != null)
        {
            d.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ActivateDivisionAsync(int id)
    {
        var d = await _context.Divisions.FindAsync(id);

        if (d != null)
        {
            d.IsActive = true;
            await _context.SaveChangesAsync();
        }
    }

    // OPTIONAL (auto generate)
    public async Task CreateDivisionsAsync(int semesterId, int maxPerDivision)
    {
        var strength = await _context.SemesterStudentStrengths
            .FirstOrDefaultAsync(s => s.SemesterId == semesterId);

        if (strength == null)
            throw new Exception("Student strength not found");

        int total = strength.TotalStudents;

        var oldDivisions = _context.Divisions
            .Where(d => d.SemesterId == semesterId);

        _context.Divisions.RemoveRange(oldDivisions);

        int divisionCount = (int)Math.Ceiling((double)total / maxPerDivision);

        var divisions = new List<Division>();

        for (int i = 0; i < divisionCount; i++)
        {
            int students = Math.Min(maxPerDivision, total - (i * maxPerDivision));

            divisions.Add(new Division
            {
                SemesterId = semesterId,
                DivisionName = ((char)('A' + i)).ToString(),
                StudentStrength = students,
                IsActive = true,
                CreatedAt = DateTime.Now
            });
        }

        _context.Divisions.AddRange(divisions);
        await _context.SaveChangesAsync();
    }
}