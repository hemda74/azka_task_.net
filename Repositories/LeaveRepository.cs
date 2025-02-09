using System.Collections.Generic;
using System.Threading.Tasks;
using task_amwag.Models;
using Microsoft.EntityFrameworkCore;
using task_amwag.Data;

namespace task_amwag.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _context;

        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Leave>> GetLeavesByEmployeeIdAsync(int employeeId)
        {
            return await _context.Leaves
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task AddLeaveAsync(Leave leave)
        {
            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeaveAsync(Leave leave)
        {
            _context.Leaves.Update(leave);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeaveAsync(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave != null)
            {
                _context.Leaves.Remove(leave);
                await _context.SaveChangesAsync();
            }
        }

       

        public async Task<Leave> GetLeaveByIdAsync(int id)
        {
            return await _context.Leaves.FindAsync(id);
        }

        public async Task<bool> AnyLeaveOverlapping(Leave newLeave)
        {
            return await _context.Leaves.AnyAsync(l =>
                l.EmployeeId == newLeave.EmployeeId &&
                l.Id != newLeave.Id &&
                (
                    (newLeave.StartDate >= l.StartDate && newLeave.StartDate < l.StartDate.AddDays(l.Duration)) ||
                    (l.StartDate >= newLeave.StartDate && l.StartDate < newLeave.StartDate.AddDays(newLeave.Duration))
                ));
        }
        public async Task<int> GetAnnualLeaveCount(int employeeId, int year)
        {
            return await _context.Leaves
                .Where(l => l.EmployeeId == employeeId && l.StartDate.Year == year)
                .SumAsync(l => l.Duration);
        }

    
        public async Task UpdateEmployeeLeaveCount(int employeeId, int totalLeaveDays)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                employee.TotalLeaveDays = totalLeaveDays;
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
            }
        }

    }
}
