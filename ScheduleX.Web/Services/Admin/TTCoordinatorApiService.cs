//using System.Net.Http.Json;
//using ScheduleX.Core.Entities;
//using ScheduleX.Web.DTOs;
//using static System.Net.WebRequestMethods;


//namespace ScheduleX.Web.Services.Admin;

//public class TTCoordinatorApiService
//{
//    private readonly HttpClient _http;

//    public TTCoordinatorApiService(HttpClient http)
//    {
//        _http = http;
//    }

//    public async Task<List<User>> GetAllAsync()
//        => await _http.GetFromJsonAsync<List<User>>("api/admin/ttcoordinator") ?? new();

//    public async Task<List<Department>> GetDepartmentsAsync()
//        => await _http.GetFromJsonAsync<List<Department>>("api/admin/ttcoordinator/departments") ?? new();

//    public async Task<List<Course>> GetCoursesAsync(int deptId)
//        => await _http.GetFromJsonAsync<List<Course>>(
//            $"api/admin/ttcoordinator/courses/{deptId}") ?? new();

//    public async Task RegisterAsync(RegisterTTCoordinatorDto dto)
//    {
//        await _http.PostAsJsonAsync("api/admin/ttcoordinator", dto);
//    }

//    public async Task ToggleAsync(int id)
//        => await _http.PatchAsync($"api/admin/ttcoordinator/{id}", null);
//}
//using Microsoft.EntityFrameworkCore;
//using ScheduleX.Infrastructure.Data;
//using ScheduleX.Core.Entities;
//using ScheduleX.Web.DTOs;

//namespace ScheduleX.Web.Services.Admin;

//public class TTCoordinatorApiService
//{
//    private readonly IDbContextFactory<AppDbContext> _factory;

//    public TTCoordinatorApiService(IDbContextFactory<AppDbContext> factory)
//    {
//        _factory = factory;
//    }

//    public async Task<List<User>> GetAllAsync()
//    {
//        using var context = await _factory.CreateDbContextAsync();

//        return await context.Users
//            .Where(x => x.Role == UserRole.TTCoordinator)
//            .Include(x => x.Department)
//            .ToListAsync();
//    }

//    public async Task<List<Department>> GetDepartmentsAsync()
//    {
//        using var context = await _factory.CreateDbContextAsync();

//        return await context.Departments
//            .Where(x => x.IsActive)
//            .ToListAsync();
//    }

//    public async Task<List<Course>> GetCoursesAsync(int deptId)
//    {
//        using var context = await _factory.CreateDbContextAsync();

//        return await context.Courses
//            .Where(x => x.DepartmentId == deptId && x.IsActive)
//            .ToListAsync();
//    }

//    public async Task RegisterAsync(RegisterTTCoordinatorDto dto)
//    {
//        using var context = await _factory.CreateDbContextAsync();

//        if (!dto.DepartmentId.HasValue || !dto.CourseId.HasValue)
//            throw new Exception("Department and Course required");

//        var user = new User
//        {
//            FullName = dto.FullName,
//            Username = dto.Username,
//            Email = dto.Email,
//            Phone = dto.Phone,
//            DepartmentId = dto.DepartmentId.Value,   // ✅ FIXED
//            Role = UserRole.TTCoordinator,
//            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
//        };

//        context.Users.Add(user);

//        var mapping = new TTCoordinatorCourse
//        {
//            User = user,
//            CourseId = dto.CourseId.Value   // ✅ FIXED
//        };

//        context.TTCoordinatorCourses.Add(mapping);

//        await context.SaveChangesAsync();
//    }
//    public async Task ToggleAsync(int id)
//    {
//        using var context = await _factory.CreateDbContextAsync();

//        var user = await context.Users.FindAsync(id);
//        if (user != null)
//        {
//            user.IsActive = !user.IsActive;
//            await context.SaveChangesAsync();
//        }
//    }
//}
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Core.Entities;
using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Services.Admin;

public class TTCoordinatorApiService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly EmailService _emailService;

    public TTCoordinatorApiService(
        IDbContextFactory<AppDbContext> factory,
        EmailService emailService)
    {
        _factory = factory;
        _emailService = emailService;
    }

    //public async Task<List<User>> GetAllAsync()
    //{
    //    using var context = await _factory.CreateDbContextAsync();

    //    return await context.Users
    //        .Where(x => x.Role == UserRole.TTCoordinator)
    //        .Include(x => x.Department)
    //        .ToListAsync();
    //}
    public async Task<List<User>> GetAllAsync()
    {
        using var context = await _factory.CreateDbContextAsync();

        return await context.Users
            .Where(x => x.Role == UserRole.TTCoordinator)
            .Include(x => x.Department)
            .Include(x => x.TTCoordinatorCourses)
                .ThenInclude(tc => tc.Course)
            .ToListAsync();
    }

    //public async Task<List<Department>> GetDepartmentsAsync()
    //{
    //    using var context = await _factory.CreateDbContextAsync();

    //    return await context.Departments
    //        .Where(x => x.IsActive)
    //        .ToListAsync();
    //}
    public async Task<List<Department>> GetDepartmentsAsync()
    {
        using var context = await _factory.CreateDbContextAsync();

        var assignedCourseIds = await context.TTCoordinatorCourses
            .Select(x => x.CourseId)
            .ToListAsync();

        var departments = await context.Departments
            .Where(d => d.IsActive)
            .ToListAsync();

        var result = new List<Department>();

        foreach (var dept in departments)
        {
            var deptCourses = await context.Courses
                .Where(c => c.DepartmentId == dept.DepartmentId && c.IsActive)
                .Select(c => c.CourseId)
                .ToListAsync();

            // if any course is NOT assigned
            if (deptCourses.Any(c => !assignedCourseIds.Contains(c)))
            {
                result.Add(dept);
            }
        }

        return result;
    }

    //public async Task<List<Course>> GetCoursesAsync(int deptId)
    //{
    //    using var context = await _factory.CreateDbContextAsync();

    //    return await context.Courses
    //        .Where(x => x.DepartmentId == deptId && x.IsActive)
    //        .ToListAsync();
    //}
    public async Task<List<Course>> GetCoursesAsync(int deptId)
    {
        using var context = await _factory.CreateDbContextAsync();

        var assignedCourseIds = await context.TTCoordinatorCourses
            .Select(x => x.CourseId)
            .ToListAsync();

        return await context.Courses
            .Where(x => x.DepartmentId == deptId
                        && x.IsActive
                        && !assignedCourseIds.Contains(x.CourseId))
            .ToListAsync();
    }
    //public async Task RegisterAsync(RegisterTTCoordinatorDto dto)
    //{
    //    if (!dto.DepartmentId.HasValue || !dto.CourseId.HasValue)
    //        throw new Exception("Department and Course required");

    //    using var context = await _factory.CreateDbContextAsync();

    //    var plainPassword = dto.Password;

    //    var user = new User
    //    {
    //        FullName = dto.FullName,
    //        Username = dto.Username,
    //        Email = dto.Email,
    //        Phone = dto.Phone,
    //        DepartmentId = dto.DepartmentId.Value,
    //        Role = UserRole.TTCoordinator,
    //        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
    //    };

    //    context.Users.Add(user);

    //    var mapping = new TTCoordinatorCourse
    //    {
    //        User = user,
    //        CourseId = dto.CourseId.Value
    //    };

    //    context.TTCoordinatorCourses.Add(mapping);

    //    await context.SaveChangesAsync();

    //    await _emailService.SendEmailAsync(
    //        user.Email!,
    //        "TT Coordinator Registration",
    //        $"Registered Successfully\n\nUsername: {user.Username}\nPassword: {plainPassword}"
    //    );
    //}
    public async Task RegisterAsync(RegisterTTCoordinatorDto dto)
    {
        if (!dto.DepartmentId.HasValue || !dto.CourseId.HasValue)
            throw new Exception("Department and Course required");

        using var context = await _factory.CreateDbContextAsync();

        // ✅ Email unique check
        var emailExists = await context.Users
            .AnyAsync(x => x.Email == dto.Email);

        if (emailExists)
            throw new Exception("Email already exists.");

        // ✅ Phone unique check
        var phoneExists = await context.Users
            .AnyAsync(x => x.Phone == dto.Phone);

        if (phoneExists)
            throw new Exception("Phone number already exists.");


        // 🔴 1. Check if course already has TT coordinator
        var courseAlreadyAssigned = await context.TTCoordinatorCourses
            .AnyAsync(x => x.CourseId == dto.CourseId.Value);

        if (courseAlreadyAssigned)
            throw new Exception("This course already has a TT Coordinator.");

        // 🔴 2. Check if same username exists in different department
        var existingUser = await context.Users
            .Include(x => x.TTCoordinatorCourses)
            .FirstOrDefaultAsync(x => x.Username == dto.Username);

        if (existingUser != null)
        {
            if (existingUser.DepartmentId != dto.DepartmentId)
                throw new Exception("Coordinator cannot be assigned to multiple departments.");
        }

        var plainPassword = dto.Password;

        var user = new User
        {
            FullName = dto.FullName,
            Username = dto.Username,
            Email = dto.Email,
            Phone = dto.Phone,
            DepartmentId = dto.DepartmentId.Value,
            Role = UserRole.TTCoordinator,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        context.Users.Add(user);

        var mapping = new TTCoordinatorCourse
        {
            User = user,
            CourseId = dto.CourseId.Value
        };

        context.TTCoordinatorCourses.Add(mapping);

        await context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.Email!,
            "TT Coordinator Registration",
            $"Registered Successfully\n\nUsername: {user.Username}\nPassword: {plainPassword}"
        );
    }

    public async Task ToggleAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();

        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsActive = !user.IsActive;
            await context.SaveChangesAsync();
        }
    }
}