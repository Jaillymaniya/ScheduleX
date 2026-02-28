//namespace ScheduleX.Web.Services
//{
//    public class AuthState
//    {
//    }
//}



namespace ScheduleX.Web.Services;

using ScheduleX.Core.Entities;

public class AuthState
{
    public int? UserId { get; private set; }
    public string? FullName { get; private set; }
    public UserRole? Role { get; private set; }

    public bool IsLoggedIn => UserId.HasValue;
    public bool IsAdmin => Role == UserRole.Admin;

    public void SignIn(int userId, string fullName, UserRole role)
    {
        UserId = userId;
        FullName = fullName;
        Role = role;
    }

    public void SignOut()
    {
        UserId = null;
        FullName = null;
        Role = null;
    }
}