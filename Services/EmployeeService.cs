namespace task_amwag.Services;
using task_amwag.Models;
using task_amwag.Repositories;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync(int page, int pageSize);
    Task<Employee> GetEmployeeByIdAsync(int id);
    Task AddEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(int id);
}

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILeaveRepository _leaveRepository;
    public EmployeeService(IEmployeeRepository employeeRepository, ILeaveRepository leaveRepository)
    {
        _employeeRepository = employeeRepository;
        _leaveRepository = leaveRepository;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(int page, int pageSize)
    {
        return await _employeeRepository.GetPaginatedEmployeesAsync(page, pageSize);
    }

    public async Task<Employee> GetEmployeeByIdAsync(int id)
    {
        return await _employeeRepository.GetEmployeeByIdAsync(id);
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        await _employeeRepository.AddEmployeeAsync(employee);
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        await _employeeRepository.UpdateEmployeeAsync(employee);
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        await _employeeRepository.DeleteEmployeeAsync(id);
    }
  
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllEmployeesAsync();

        foreach (var employee in employees)
        {
            employee.TotalLeaveDays = await _leaveRepository.GetAnnualLeaveCount(employee.Id, DateTime.UtcNow.Year);
        }

        return employees;
    }



}
