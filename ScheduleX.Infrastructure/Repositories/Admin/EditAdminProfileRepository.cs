using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.Admin;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.Admin;

public class EditAdminProfileRepository : IEditAdminProfileRepository
{
    private readonly AppDbContext _context;   // ✅ Correct DbContext name

    public EditAdminProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetAdminByIdAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Role == UserRole.Admin &&
                x.IsActive);
    }

    public async Task<bool> UpdateAdminProfileAsync(User user)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == user.UserId &&
                x.Role == UserRole.Admin &&
                x.IsActive);

        if (existingUser == null)
            return false;

        existingUser.FullName = user.FullName;
        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.Phone = user.Phone;

        await _context.SaveChangesAsync();

        return true;
    }
}