public class LeaveDTO
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public int Duration { get; set; }
}