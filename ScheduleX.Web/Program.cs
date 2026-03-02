//using ScheduleX.Web.Components;
//using Microsoft.EntityFrameworkCore;
//using ScheduleX.Infrastructure.Data;
//using Microsoft.AspNetCore.Components;
//using ScheduleX.Core.Interfaces;
//using ScheduleX.Infrastructure.Repositories;
//using ScheduleX.Web.Services.Admin;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContextFactory<AppDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection")));

//// Add services to the container.
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();

//builder.Services.AddControllers();

//builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

//builder.Services.AddScoped(sp =>
//{
//    var navigation = sp.GetRequiredService<NavigationManager>();
//    return new HttpClient
//    {
//        BaseAddress = new Uri(navigation.BaseUri)
//    };
//});

//builder.Services.AddScoped<DepartmentApiService>();

////for login
//builder.Services.AddScoped<ScheduleX.Web.Services.AuthState>();

////password hash 
//builder.Services.AddScoped<ScheduleX.Web.Services.PasswordHasher>();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//app.UseStaticFiles();   //  UseStaticFiles instead of MapStaticAssets
//app.UseRouting();       //  Routing first

//app.UseAntiforgery();   //  MUST come AFTER UseRouting

//app.MapControllers();   //  Map API controllers

//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();

//app.Run();




using ScheduleX.Web.Components;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using ScheduleX.Core.Interfaces;
using ScheduleX.Infrastructure.Repositories;
using ScheduleX.Web.Services.Admin;

// ✅ NEW (for change password)
using ScheduleX.Core.Interfaces.Admin;
using ScheduleX.Infrastructure.Repositories.Admin;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

// ✅ REGISTER CHANGE PASSWORD REPOSITORY
builder.Services.AddScoped<IChangePasswordRepository, ChangePasswordRepository>();

builder.Services.AddScoped(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navigation.BaseUri)
    };
});

builder.Services.AddScoped<DepartmentApiService>();

// Login state
builder.Services.AddScoped<ScheduleX.Web.Services.AuthState>();

// Password hasher
builder.Services.AddScoped<ScheduleX.Web.Services.PasswordHasher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();   // Static files
app.UseRouting();       // Routing first

app.UseAntiforgery();   // Must come AFTER UseRouting

app.MapControllers();   // Map API controllers

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

