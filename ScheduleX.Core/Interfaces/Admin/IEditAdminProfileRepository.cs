using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.Admin;

public interface IEditAdminProfileRepository
{
    Task<User?> GetAdminByIdAsync(int userId);
    Task<bool> UpdateAdminProfileAsync(User user);
}