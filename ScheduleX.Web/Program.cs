

////this is workable
//using ScheduleX.Web.Components;
//using Microsoft.EntityFrameworkCore;
//using ScheduleX.Infrastructure.Data;
//using Microsoft.AspNetCore.Components;

//using ScheduleX.Core.Interfaces;
//using ScheduleX.Infrastructure.Repositories;

//using ScheduleX.Web.Services.Admin;
//using Timetable.Infrastructure.Repositories;

//using ScheduleX.Core.Interfaces.TTCoordinator;
//using ScheduleX.Infrastructure.Repositories.TTCoordinator;

//using ScheduleX.Core.Interfaces.Admin;
//using ScheduleX.Infrastructure.Repositories.Admin;

//using ScheduleX.Web.Services.Excel;
//using ScheduleX.Web.Services.TimeTable;

//var builder = WebApplication.CreateBuilder(args);

//// ================= DB =================
//builder.Services.AddDbContextFactory<AppDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection")
//    )
//);

//// ================= BLAZOR =================
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();

//builder.Services.AddServerSideBlazor()
//    .AddCircuitOptions(options =>
//    {
//        options.DetailedErrors = true;
//    });

//// ================= CONTROLLERS =================
//builder.Services.AddControllers();

//// ================= SERVICES =================
//builder.Services.AddScoped<EmailService>();
//builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();

//// ================= ADMIN REPOSITORIES =================
//builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
//builder.Services.AddScoped<ICourseRepository, CourseRepository>();
//builder.Services.AddScoped<IChangePasswordRepository, ChangePasswordRepository>();
//builder.Services.AddScoped<IEditAdminProfileRepository, EditAdminProfileRepository>();

//// ================= TT COORDINATOR REPOSITORIES =================
//builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
//builder.Services.AddScoped<IRoomRepository, RoomRepository>();
//builder.Services.AddScoped<IScheduleConfigRepository, ScheduleConfigRepository>();
//builder.Services.AddScoped<IDivisionService, DivisionService>();
//builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
//builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
//builder.Services.AddScoped<ISubjectSemesterRepository, SubjectSemesterRepository>();
//builder.Services.AddScoped<ISubjectFacultyRepository, SubjectFacultyRepository>();
//builder.Services.AddScoped<IDivisionRoomAllocationRepository, DivisionRoomAllocationRepository>();

//// ================= HTTP CLIENT =================
//builder.Services.AddScoped(sp =>
//{
//    var navigation = sp.GetRequiredService<NavigationManager>();
//    return new HttpClient
//    {
//        BaseAddress = new Uri(navigation.BaseUri)
//    };
//});

//// ================= API SERVICES =================
//builder.Services.AddScoped<TTCoordinatorApiService>();
//builder.Services.AddScoped<DepartmentApiService>();
//builder.Services.AddScoped<CourseApiService>();

//// ================= OTHER SERVICES =================
//builder.Services.AddScoped<IExcelService, ExcelService>();
//builder.Services.AddScoped<ITimeTableService, TimeTableService>();

//// ================= AUTH SERVICES (⚠️ REMOVE LATER) =================
//builder.Services.AddScoped<ScheduleX.Web.Services.AuthState>(); // ❌ should be removed
//builder.Services.AddScoped<ScheduleX.Web.Services.PasswordHasher>();

//// ================= SESSION =================
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession();

//builder.Services.AddScoped<HttpClient>(sp =>
//{
//    return new HttpClient
//    {
//        BaseAddress = new Uri("https://localhost:7299/")
//    };
//});

//var app = builder.Build();

//// ================= ERROR HANDLING =================
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    app.UseHsts();
//}

//// ================= MIDDLEWARE =================
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();

//app.UseSession();
//app.UseAntiforgery();

//// ================= MAP =================
//app.MapControllers();

//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();

//app.Run();





//for edit


//this is workable
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

using ScheduleX.Web.Services.Excel;
using ScheduleX.Web.Services.TimeTable;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
// ✅ ONLY FACTORY (IMPORTANT)
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ================= BLAZOR =================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

// ================= CONTROLLERS =================
builder.Services.AddControllers();

// ================= SERVICES =================
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();

// ================= ADMIN =================
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IChangePasswordRepository, ChangePasswordRepository>();
builder.Services.AddScoped<IEditAdminProfileRepository, EditAdminProfileRepository>();

// ================= TT =================
builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IScheduleConfigRepository, ScheduleConfigRepository>();
builder.Services.AddScoped<IDivisionService, DivisionService>();
builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectSemesterRepository, SubjectSemesterRepository>();
builder.Services.AddScoped<ISubjectFacultyRepository, SubjectFacultyRepository>();
builder.Services.AddScoped<IDivisionRoomAllocationRepository, DivisionRoomAllocationRepository>();

// ================= HTTP =================
builder.Services.AddScoped(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navigation.BaseUri)
    };
});

builder.Services.AddScoped<ProtectedSessionStorage>();
// ================= API SERVICES =================
builder.Services.AddScoped<TTCoordinatorApiService>();
builder.Services.AddScoped<DepartmentApiService>();
builder.Services.AddScoped<CourseApiService>();

// ================= OTHER =================
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ITimeTableService, TimeTableService>();

// ================= AUTH =================
builder.Services.AddScoped<ScheduleX.Web.Services.AuthState>();
builder.Services.AddScoped<ScheduleX.Web.Services.PasswordHasher>();

// ================= SESSION =================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// ================= ERROR =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// ================= MIDDLEWARE =================
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAntiforgery();

// ================= MAP =================
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();