
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
    // ADD (FIXED WITH ERROR LOG)
    // ===============================
    public async Task AddAsync(Faculty faculty)
    {
        try
        {
            faculty.IsActive = true;
            faculty.CreatedAt = DateTime.Now;

            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Faculty Inserted ID: " + faculty.FacultyId);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ ERROR INSERTING FACULTY: " + ex.InnerException?.Message ?? ex.Message);
            throw;
        }
    }

    // ===============================
    // GET ALL
    // ===============================
    //public async Task<List<Faculty>> GetAllAsync()
    //{
    //    return await _context.Faculties
    //        .Include(f => f.Department)
    //        .OrderByDescending(f => f.IsActive)
    //        .ThenBy(f => f.FacultyName)
    //        .ToListAsync();
    //}


    public async Task<List<Faculty>> GetAllAsync()
    {
        return await _context.Faculties
            .Include(f => f.Department)
            .Include(f => f.ExternalPermissions)                 // 🔥 ADD
                .ThenInclude(ep => ep.Department)                // 🔥 ADD
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
    // UPDATE
    // ===============================
    //public async Task UpdateAsync(Faculty faculty)
    //{
    //    var existing = await _context.Faculties
    //        .FirstOrDefaultAsync(f => f.FacultyId == faculty.FacultyId);

    //    if (existing != null)
    //    {
    //        existing.FacultyName = faculty.FacultyName;
    //        existing.FacultyCode = faculty.FacultyCode;
    //        existing.Email = faculty.Email;
    //        existing.Phone = faculty.Phone;
    //        existing.DepartmentId = faculty.DepartmentId;
    //        existing.MaxLecturesPerDay = faculty.MaxLecturesPerDay;
    //        existing.IsExternal = faculty.IsExternal;

    //        await _context.SaveChangesAsync();
    //    }
    //}

    //public async Task UpdateAsync(Faculty faculty)
    //{
    //    var existing = await _context.Faculties
    //        .FirstOrDefaultAsync(f => f.FacultyId == faculty.FacultyId);

    //    if (existing != null)
    //    {
    //        existing.FacultyName = faculty.FacultyName;
    //        existing.FacultyCode = faculty.FacultyCode;
    //        existing.Email = faculty.Email;
    //        existing.Phone = faculty.Phone;
    //        existing.DepartmentId = faculty.DepartmentId;
    //        existing.MaxLecturesPerDay = faculty.MaxLecturesPerDay;
    //        existing.IsExternal = faculty.IsExternal;

    //        await _context.SaveChangesAsync();

    //        // 🔥 HANDLE EXTERNAL TABLE
    //        var existingExternal = await _context.ExternalFacultyPermissions
    //            .FirstOrDefaultAsync(x => x.FacultyId == faculty.FacultyId);

    //        if (faculty.IsExternal)
    //        {
    //            if (existingExternal == null)
    //            {
    //                _context.ExternalFacultyPermissions.Add(new ExternalFacultyPermission
    //                {
    //                    FacultyId = faculty.FacultyId,
    //                    DepartmentId = faculty.DepartmentId, // ⚠ you can change if needed
    //                    IsActive = true
    //                });
    //            }
    //        }
    //        else
    //        {
    //            if (existingExternal != null)
    //            {
    //                _context.ExternalFacultyPermissions.Remove(existingExternal);
    //            }
    //        }

    //        await _context.SaveChangesAsync();
    //    }
    //}


    //iscorrect
    //public async Task UpdateAsync(Faculty faculty)
    //{
    //    var existing = await _context.Faculties
    //        .FirstOrDefaultAsync(f => f.FacultyId == faculty.FacultyId);

    //    if (existing != null)
    //    {
    //        existing.FacultyName = faculty.FacultyName;
    //        existing.FacultyCode = faculty.FacultyCode;
    //        existing.Email = faculty.Email;
    //        existing.Phone = faculty.Phone;
    //        existing.DepartmentId = faculty.DepartmentId;
    //        existing.MaxLecturesPerDay = faculty.MaxLecturesPerDay;
    //        existing.IsExternal = faculty.IsExternal;

    //        await _context.SaveChangesAsync();

    //        // 🔥 HANDLE EXTERNAL TABLE
    //        var existingExternal = await _context.ExternalFacultyPermissions
    //            .FirstOrDefaultAsync(x => x.FacultyId == faculty.FacultyId);

    //        if (faculty.IsExternal)
    //        {
    //            if (existingExternal == null)
    //            {
    //                // 🔥 INSERT (main requirement)
    //                _context.ExternalFacultyPermissions.Add(new ExternalFacultyPermission
    //                {
    //                    FacultyId = faculty.FacultyId,
    //                    DepartmentId = faculty.DepartmentId, // temp (will override below)
    //                    IsActive = true
    //                });
    //            }
    //        }
    //        else
    //        {
    //            if (existingExternal != null)
    //            {
    //                _context.ExternalFacultyPermissions.Remove(existingExternal);
    //            }
    //        }

    //        await _context.SaveChangesAsync();
    //    }
    //}

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
            existing.DepartmentId = faculty.DepartmentId; // ✔ main dept only
            existing.MaxLecturesPerDay = faculty.MaxLecturesPerDay;
            existing.IsExternal = faculty.IsExternal;

            await _context.SaveChangesAsync();
        }
    }


    // ===============================
    // DELETE
    // ===============================
    //public async Task SoftDeleteAsync(int id)
    //{
    //    var faculty = await _context.Faculties.FindAsync(id);

    //    if (faculty != null)
    //    {
    //        faculty.IsActive = false;
    //        await _context.SaveChangesAsync();
    //    }
    //}


    public async Task SoftDeleteAsync(int id)
    {
        var faculty = await _context.Faculties.FindAsync(id);

        if (faculty != null)
        {
            faculty.IsActive = false;

            // 🔥 REMOVE EXTERNAL
            var external = await _context.ExternalFacultyPermissions
                .Where(x => x.FacultyId == id)
                .ToListAsync();

            _context.ExternalFacultyPermissions.RemoveRange(external);

            await _context.SaveChangesAsync();
        }
    }

    // ===============================
    // ACTIVATE
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

    // ===============================
    // EXTERNAL
    // ===============================
    public async Task AddExternalPermissionAsync(ExternalFacultyPermission permission)
    {
        try
        {
            _context.ExternalFacultyPermissions.Add(permission);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ External Permission Inserted");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ ERROR INSERTING EXTERNAL: " + ex.InnerException?.Message ?? ex.Message);
            throw;
        }
    }

    public async Task<List<ExternalFacultyPermission>> GetExternalPermissions(int facultyId)
    {
        return await _context.ExternalFacultyPermissions
            .Where(x => x.FacultyId == facultyId)
            .ToListAsync();
    }
}