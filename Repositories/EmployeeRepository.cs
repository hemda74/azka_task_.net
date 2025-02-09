using System.Collections.Generic;
using System.Threading.Tasks;
using task_amwag.Models;
using Microsoft.EntityFrameworkCore;
using task_amwag.Data;

namespace task_amwag.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.Include(e => e.Leaves).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.Include(e => e.Leaves).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Employee>> GetPaginatedEmployeesAsync(int page, int pageSize)
        {
            return await _context.Employees
                .Include(e => e.Leaves)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
