using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator;
//{
//    internal class ISemesterRepository
//    {
//    }
//}


public interface ISemesterRepository
{
    Task<List<Semester>> GetAllAsync();
    Task<List<Semester>> GetByCourseAsync(int courseId);
    Task AddAsync(Semester semester);
    Task UpdateAsync(Semester semester);
    Task ToggleStatusAsync(int id);
}