using System.Threading.Tasks;

namespace ScheduleX.Core.Interfaces.Admin;

public interface IChangePasswordRepository
{
    Task<bool> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword);
}