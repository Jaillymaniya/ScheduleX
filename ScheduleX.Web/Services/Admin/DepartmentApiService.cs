using System.Net.Http.Json;
using ScheduleX.Core.Entities;

namespace ScheduleX.Web.Services.Admin;

public class DepartmentApiService
{
    private readonly HttpClient _http;

    public DepartmentApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Department>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<Department>>("api/admin/department") ?? new();

    public async Task CreateAsync(Department dept)
        => await _http.PostAsJsonAsync("api/admin/department", dept);

    public async Task UpdateAsync(Department dept)
        => await _http.PutAsJsonAsync($"api/admin/department/{dept.DepartmentId}", dept);

    public async Task ToggleAsync(int id)
        => await _http.PatchAsync($"api/admin/department/{id}", null);
}