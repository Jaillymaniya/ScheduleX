////namespace ScheduleX.Web.Services
////{
////    public class AuthState
////    {
////    }
////}



//namespace ScheduleX.Web.Services;

//using ScheduleX.Core.Entities;

//public class AuthState
//{
//    public int? UserId { get; private set; }
//    public string? FullName { get; private set; }
//    public UserRole? Role { get; private set; }

//    public bool IsLoggedIn => UserId.HasValue;
//    public bool IsAdmin => Role == UserRole.Admin;

//    public void SignIn(int userId, string fullName, UserRole role)
//    {
//        UserId = userId;
//        FullName = fullName;
//        Role = role;
//    }

//    public void SignOut()
//    {
//        UserId = null;
//        FullName = null;
//        Role = null;
//    }
//}

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ScheduleX.Core.Entities;
namespace ScheduleX.Web.Services
{
    public class AuthState
    {
        private readonly ProtectedSessionStorage _session;

        public int? UserId { get; private set; }
        public string? FullName { get; private set; }
        public UserRole? Role { get; private set; }

        public bool IsLoggedIn => UserId.HasValue;
        public bool IsAdmin => Role == UserRole.Admin;

        public AuthState(ProtectedSessionStorage session)
        {
            _session = session;
        }

        public async Task SignIn(int userId, string fullName, UserRole role)
        {
            UserId = userId;
            FullName = fullName;
            Role = role;

            await _session.SetAsync("userId", userId);
            await _session.SetAsync("fullName", fullName);
            await _session.SetAsync("role", role.ToString());
        }

        public async Task LoadFromSession()
        {
            var userId = await _session.GetAsync<int>("userId");
            var fullName = await _session.GetAsync<string>("fullName");
            var role = await _session.GetAsync<string>("role");

            if (userId.Success)
                UserId = userId.Value;

            if (fullName.Success)
                FullName = fullName.Value;

            if (role.Success && Enum.TryParse<UserRole>(role.Value, out var parsedRole))
                Role = parsedRole;
        }

        public async Task SignOut()
        {
            UserId = null;
            FullName = null;
            Role = null;

            await _session.DeleteAsync("userId");
            await _session.DeleteAsync("fullName");
            await _session.DeleteAsync("role");
        }
    }
}
