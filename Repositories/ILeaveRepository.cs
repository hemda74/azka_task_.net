using System.Collections.Generic;
using System.Threading.Tasks;
using task_amwag.Models;

namespace task_amwag.Repositories
{
    public interface ILeaveRepository
    {
        Task<IEnumerable<Leave>> GetLeavesByEmployeeIdAsync(int employeeId);
        Task AddLeaveAsync(Leave leave);
        Task UpdateLeaveAsync(Leave leave);
        Task DeleteLeaveAsync(int id);
        Task<bool> AnyLeaveOverlapping(Leave leave);
        Task<int> GetAnnualLeaveCount(int employeeId, int year);
        Task<Leave> GetLeaveByIdAsync(int id);
        Task UpdateEmployeeLeaveCount(int employeeId, int totalLeaveDays);

     
    }
}
