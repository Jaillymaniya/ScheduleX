//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ScheduleX.Core.Interfaces.Admin
//{
//    internal class ICourseRepository
//    {
//    }
//}


using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<List<Course>> GetByDepartmentAsync(int departmentId);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task ToggleStatusAsync(int courseId);
}