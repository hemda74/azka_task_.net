namespace task_amwag.Controllers;

using Microsoft.AspNetCore.Mvc;
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
    [HttpPost]
    public async Task<IActionResult> CreateLeave([FromBody] Leave leave)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _leaveService.AddLeaveAsync(leave);

        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });  
        }

        return CreatedAtAction(nameof(GetLeavesByEmployeeId), new { employeeId = leave.EmployeeId }, response.Data);
    }



    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchLeave(int id, [FromBody] Dictionary<string, object> updates)
    {
        var leave = await _leaveService.GetLeaveByIdAsync(id);
        if (leave == null)
        {
            return NotFound("Leave not found.");
        }

        foreach (var update in updates)
        {
            var propertyInfo = typeof(Leave).GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(leave, Convert.ChangeType(update.Value, propertyInfo.PropertyType));
            }
        }

        await _leaveService.UpdateLeaveAsync(leave);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        await _leaveService.DeleteLeaveAsync(id);
        return NoContent();
    }
}
