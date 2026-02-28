//namespace ScheduleX.Web.Services
//{
//    public class PasswordHasher
//    {
//    }
//}


namespace ScheduleX.Web.Services;

public class PasswordHasher
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}