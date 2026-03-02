using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.Admin;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.Admin;

public class ChangePasswordRepository : IChangePasswordRepository
{
    private readonly AppDbContext _context;

    public ChangePasswordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Role == UserRole.Admin &&
                x.IsActive);

        if (user == null)
            return false;

        // ✅ Verify current password using BCrypt
        bool isValid = BCrypt.Net.BCrypt.Verify(
            currentPassword,
            user.PasswordHash);

        if (!isValid)
            return false;

        // ✅ Hash new password using BCrypt
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

        await _context.SaveChangesAsync();

        return true;
    }
}