using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;
namespace ScheduleX.Core.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();
    Task AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task ToggleStatusAsync(int id);
}