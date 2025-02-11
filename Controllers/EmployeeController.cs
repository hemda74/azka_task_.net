using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_amwag.Models;
using task_amwag.Services;
using System;
namespace task_amwag.Controllers
{
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

            var totalCount = await _employeeService.GetTotalEmployeesCountAsync();
            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");

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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "بيانات الموظف غير صحيحة"
                    });
                }

                var existingEmployee = await _employeeService.GetEmployeeByNameAsync(employee.Name);
                if (existingEmployee != null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "يوجد موظف بهذا الاسم بالفعل"
                    });
                }

                await _employeeService.AddEmployeeAsync(employee);
                return Ok(new
                {
                    success = true,
                    message = "تم إضافة الموظف بنجاح",
                    data = employee
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Employees_Name") == true)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "يوجد موظف بهذا الاسم بالفعل"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "حدث خطأ أثناء إضافة الموظف"
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEmployee(int id, [FromBody] Dictionary<string, object> updates)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "الموظف غير موجود"
                    });
                }

                if (updates.ContainsKey("name"))
                {
                    var newName = updates["name"].ToString();
                    var existingEmployee = await _employeeService.GetEmployeeByNameAsync(newName);
                    if (existingEmployee != null && existingEmployee.Id != id)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "يوجد موظف بهذا الاسم بالفعل"
                        });
                    }
                }

                bool hasChanges = false;

                foreach (var update in updates)
                {
                    switch (update.Key.ToLower())
                    {
                        case "name":
                            var newName = update.Value.ToString();
                            if (employee.Name != newName)
                            {
                                employee.Name = newName;
                                hasChanges = true;
                            }
                            break;

                        case "dateofbirth":
                            if (DateTime.TryParse(update.Value.ToString(), out DateTime newDate))
                            {
                                if (employee.DateOfBirth != newDate)
                                {
                                    employee.DateOfBirth = newDate;
                                    hasChanges = true;
                                }
                            }
                            else
                            {
                                return BadRequest(new
                                {
                                    success = false,
                                    message = "صيغة التاريخ غير صحيحة"
                                });
                            }
                            break;

                        case "qualification":
                            var newQualification = update.Value.ToString();
                            if (employee.Qualification != newQualification)
                            {
                                employee.Qualification = newQualification;
                                hasChanges = true;
                            }
                            break;
                    }
                }

                if (hasChanges)
                {
                    await _employeeService.UpdateEmployeeAsync(employee);
                    var updatedEmployee = await _employeeService.GetEmployeeByIdAsync(id);
                    return Ok(new
                    {
                        success = true,
                        message = "تم تحديث بيانات الموظف بنجاح",
                        data = updatedEmployee
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "لم يتم إجراء أي تغييرات"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "حدث خطأ أثناء تحديث بيانات الموظف"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "الموظف غير موجود"
                    });
                }

                await _employeeService.DeleteEmployeeAsync(id);
                return Ok(new
                {
                    success = true,
                    message = "تم حذف الموظف بنجاح"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "حدث خطأ أثناء حذف الموظف"
                });
            }
        }
    }
}