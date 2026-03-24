




using ScheduleX.Web.Components;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using Microsoft.AspNetCore.Components;

using ScheduleX.Core.Interfaces;
using ScheduleX.Infrastructure.Repositories;

using ScheduleX.Web.Services.Admin;
using Timetable.Infrastructure.Repositories;

using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Repositories.TTCoordinator;

using ScheduleX.Core.Interfaces.Admin;
using ScheduleX.Infrastructure.Repositories.Admin;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Repositories.TTCoordinator;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();


// ================= ADMIN REPOSITORIES =================
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IChangePasswordRepository, ChangePasswordRepository>();
builder.Services.AddScoped<IEditAdminProfileRepository, EditAdminProfileRepository>();

// ================= TT COORDINATOR REPOSITORIES =================
builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();   // ROOM MANAGEMENT

// ================= HTTP CLIENT =================
builder.Services.AddScoped(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navigation.BaseUri)
    };
});

// ================= API SERVICES =================
builder.Services.AddScoped<TTCoordinatorApiService>();
builder.Services.AddScoped<DepartmentApiService>();
builder.Services.AddScoped<CourseApiService>();
  // ROOM API SERVICE

// ================= AUTH SERVICES =================
builder.Services.AddScoped<ScheduleX.Web.Services.AuthState>();
builder.Services.AddScoped<ScheduleX.Web.Services.PasswordHasher>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAntiforgery();

app.UseStaticFiles();   // Static files
app.UseRouting();       // Routing first

app.UseAntiforgery();   // Must come AFTER UseRouting

app.MapControllers();   // Map API controllers


// ================= MAP CONTROLLERS =================
app.MapControllers();

// ================= RAZOR COMPONENTS =================
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();



