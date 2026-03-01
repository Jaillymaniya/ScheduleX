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
//builder.Services.AddScoped<TTCoordinatorApiService>();//ttcoordinator register by admin
////email 
//builder.Services.AddScoped<EmailService>();

//builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();//department


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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

//  SINGLE HttpClient Registration (IMPORTANT)
builder.Services.AddScoped(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navigation.BaseUri)
    };
});

builder.Services.AddScoped<TTCoordinatorApiService>();
builder.Services.AddScoped<DepartmentApiService>();

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

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();