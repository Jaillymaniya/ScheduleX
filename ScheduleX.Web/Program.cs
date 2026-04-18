




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
//using ScheduleX.Core.Interfaces.TTCoordinator;
//using ScheduleX.Infrastructure.Repositories.TTCoordinator;

using ScheduleX.Web.Services.Excel;
using ScheduleX.Web.Services.TimeTable;

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
builder.Services.AddScoped<IScheduleConfigRepository, ScheduleConfigRepository>();
builder.Services.AddScoped<IDivisionService, DivisionService>();
builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectSemesterRepository, SubjectSemesterRepository>();
builder.Services.AddScoped<ISubjectFacultyRepository, SubjectFacultyRepository>();
builder.Services.AddScoped<IDivisionRoomAllocationRepository, DivisionRoomAllocationRepository>();
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
// ================= SERVICES =================
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ITimeTableService, TimeTableService>();
// ROOM API SERVICE

// ================= AUTH SERVICES =================
builder.Services.AddScoped<ScheduleX.Web.Services.AuthState>();
builder.Services.AddScoped<ScheduleX.Web.Services.PasswordHasher>();

// ================== forgot password =================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddScoped<HttpClient>(sp =>
{
    return new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7299/")
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();   // Static files
app.UseRouting();       // Routing first

app.UseSession();

app.UseAntiforgery();   // Must come AFTER UseRouting


// ================= MAP CONTROLLERS =================
app.MapControllers();

// ================= RAZOR COMPONENTS =================
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();



