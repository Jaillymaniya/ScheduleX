using ScheduleX.Web.Components;
using Microsoft.EntityFrameworkCore;
using Timetable.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

builder.Services.AddScoped(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navigation.BaseUri)
    };
});

builder.Services.AddScoped<DepartmentApiService>();

//for login
builder.Services.AddScoped<ScheduleX.Web.Services.AuthState>();

//password hash 
builder.Services.AddScoped<ScheduleX.Web.Services.PasswordHasher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();   //  UseStaticFiles instead of MapStaticAssets
app.UseRouting();       //  Routing first

app.UseAntiforgery();   //  MUST come AFTER UseRouting

app.MapControllers();   //  Map API controllers

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


