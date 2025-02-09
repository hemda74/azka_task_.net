using task_amwag.Models;
using task_amwag.Repositories;

namespace task_amwag.Services
{
    public interface ILeaveService
    {
        Task<IEnumerable<Leave>> GetLeavesByEmployeeIdAsync(int employeeId);
        Task<ServiceResponse<Leave>> AddLeaveAsync(Leave leave);
        Task UpdateLeaveAsync(Leave leave);
        Task DeleteLeaveAsync(int id);
        Task<bool> IsLeaveOverlapping(Leave leave);
        Task<int> GetAnnualLeaveCount(int employeeId, int year);
        Task UpdateEmployeeLeaveCount(int employeeId);
        Task<Leave> GetLeaveByIdAsync(int id);
    }

    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;

        public LeaveService(ILeaveRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
        }

        public async Task<ServiceResponse<Leave>> AddLeaveAsync(Leave leave)
        {
            var response = new ServiceResponse<Leave>();

            if (await IsLeaveOverlapping(leave))
            {
                response.Success = false;
                response.Message = "لا يمكن تسجيل لنفس الموظف اجازتين في نفس الفترة";
                return response;
            }

            int usedLeaveDays = await GetAnnualLeaveCount(leave.EmployeeId, leave.StartDate.Year);
            if ((usedLeaveDays + leave.Duration) > 30)
            {
                response.Success = false;
                response.Message = "لا يمكن تجاوز 30 يومًا من الإجازة في السنة";
                return response;
            }

            await _leaveRepository.AddLeaveAsync(leave);
            await UpdateEmployeeLeaveCount(leave.EmployeeId);
            response.Data = leave;
            response.Success = true;
            response.Message = "تم تسجيل الإجازة بنجاح";
            return response;
        }

        public async Task<bool> IsLeaveOverlapping(Leave newLeave)
        {
            return await _leaveRepository.AnyLeaveOverlapping(newLeave);
        }

        public async Task<int> GetAnnualLeaveCount(int employeeId, int year)
        {
            return await _leaveRepository.GetAnnualLeaveCount(employeeId, year);
        }

        public async Task<IEnumerable<Leave>> GetLeavesByEmployeeIdAsync(int employeeId)
        {
            return await _leaveRepository.GetLeavesByEmployeeIdAsync(employeeId);
        }

        public async Task UpdateLeaveAsync(Leave leave)
        {
            await _leaveRepository.UpdateLeaveAsync(leave);
            await UpdateEmployeeLeaveCount(leave.EmployeeId);
        }

        public async Task DeleteLeaveAsync(int id)
        {
            var leave = await _leaveRepository.GetLeaveByIdAsync(id);
            if (leave != null)
            {
                await _leaveRepository.DeleteLeaveAsync(id);
                await UpdateEmployeeLeaveCount(leave.EmployeeId);
            }
        }

        public async Task<Leave> GetLeaveByIdAsync(int id)
        {
            return await _leaveRepository.GetLeaveByIdAsync(id);
        }

        public async Task UpdateEmployeeLeaveCount(int employeeId)
        {
            int totalLeaveDays = await GetAnnualLeaveCount(employeeId, System.DateTime.Now.Year);
            await _leaveRepository.UpdateEmployeeLeaveCount(employeeId, totalLeaveDays);
        }
    }
}
