namespace task_amwag.Controllers;
using Microsoft.AspNetCore.Mvc;
using task_amwag.Models;
using task_amwag.Services;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var employees = await _employeeService.GetAllEmployeesAsync(page, pageSize);
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _employeeService.AddEmployeeAsync(employee);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchEmployee(int id, [FromBody] Dictionary<string, object> updates)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound("Employee not found.");
        }

        foreach (var update in updates)
        {
            var propertyInfo = typeof(Employee).GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(employee, Convert.ChangeType(update.Value, propertyInfo.PropertyType));
            }
        }

        await _employeeService.UpdateEmployeeAsync(employee);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        await _employeeService.DeleteEmployeeAsync(id);
        return NoContent();
    }
}
