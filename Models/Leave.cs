namespace task_amwag.Models;
using System.Text.Json.Serialization;
public class Leave
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public int Duration { get; set; }
    [JsonIgnore]
    public Employee? Employee { get; set; }
}