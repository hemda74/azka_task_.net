using System.Collections.Generic;
using System.Threading.Tasks;
using task_amwag.Models;

namespace task_amwag.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> GetPaginatedEmployeesAsync(int page, int pageSize);
      

    }
}
