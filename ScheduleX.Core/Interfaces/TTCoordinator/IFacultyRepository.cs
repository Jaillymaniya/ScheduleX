
using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator;

public interface IFacultyRepository
{
    Task AddAsync(Faculty faculty);
    Task<List<Faculty>> GetAllAsync();
    Task<List<Department>> GetDepartmentsAsync();

    Task UpdateAsync(Faculty faculty);
    Task SoftDeleteAsync(int id);
    Task ActivateAsync(int id);

    // External
    Task AddExternalPermissionAsync(ExternalFacultyPermission permission);
    Task<List<ExternalFacultyPermission>> GetExternalPermissions(int facultyId);
}