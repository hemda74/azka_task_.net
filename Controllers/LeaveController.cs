namespace task_amwag.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using task_amwag.Models;
using task_amwag.Services;

[Route("api/[controller]")]
[ApiController]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }


    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<Leave>>> GetLeavesByEmployeeId(int employeeId)
    {
        var leaves = await _leaveService.GetLeavesByEmployeeIdAsync(employeeId);
        return Ok(leaves);
    }
    [HttpPost]
    public async Task<IActionResult> CreateLeave([FromBody] Leave leave)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "بيانات الإجازة غير صحيحة" });
        }

        var response = await _leaveService.AddLeaveAsync(leave);

        if (!response.Success)
        {
            return BadRequest(new { success = false, message = response.Message });
        }

        return Ok(new
        {
            success = true,
            message = "تم إضافة الإجازة بنجاح",
            data = response.Data
        });
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchLeave(int id, [FromBody] Dictionary<string, object> updates)
    {
        try
        {
            var leave = await _leaveService.GetLeaveByIdAsync(id);
            if (leave == null)
            {
                return NotFound(new { success = false, message = "الإجازة غير موجودة" });
            }

            bool hasChanges = false;

          
            foreach (var update in updates)
            {
                switch (update.Key.ToLower())
                {
                    case "startdate":
                        if (DateTime.TryParse(update.Value.ToString(), out DateTime newStartDate))
                        {
                            if (leave.StartDate != newStartDate)
                            {
                                leave.StartDate = newStartDate;
                                hasChanges = true;
                                Console.WriteLine($"StartDate updated to {newStartDate}");
                            }
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "صيغة التاريخ غير صحيحة" });
                        }
                        break;

                    case "duration":
                        if (int.TryParse(update.Value.ToString(), out int newDuration))
                        {
                            if (leave.Duration != newDuration)
                            {
                                leave.Duration = newDuration;
                                hasChanges = true;
                                Console.WriteLine($"Duration updated to {newDuration}");
                            }
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "صيغة المدة غير صحيحة" });
                        }
                        break;

                    case "leavetype":
                        var newLeaveType = update.Value.ToString();
                        if (leave.LeaveType != newLeaveType)
                        {
                            leave.LeaveType = newLeaveType;
                            hasChanges = true;
                            Console.WriteLine($"LeaveType updated to {newLeaveType}");
                        }
                        break;
                }
            }

            if (hasChanges)
            {
                try
                {
                    await _leaveService.UpdateLeaveAsync(leave);

                
                    var updatedLeave = await _leaveService.GetLeaveByIdAsync(id);

                    return Ok(new
                    {
                        success = true,
                        message = "تم تحديث الإجازة بنجاح",
                        data = updatedLeave
                    });
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Database update error: {ex}");
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "فشل تحديث الإجازة في قاعدة البيانات",
                        error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving changes: {ex}");
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "فشل تحديث الإجازة",
                        error = ex.Message
                    });
                }
            }

            return Ok(new { success = true, message = "لم يتم إجراء أي تغييرات" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex}");
            return StatusCode(500, new
            {
                success = false,
                message = "حدث خطأ أثناء تحديث الإجازة",
                error = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        try
        {
            await _leaveService.DeleteLeaveAsync(id);
            return Ok(new { success = true, message = "تم حذف الإجازة بنجاح" });
        }
        catch (Exception)
        {
            return BadRequest(new { success = false, message = "فشل حذف الإجازة" });
        }
    }
}