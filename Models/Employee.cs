    namespace task_amwag.Models;
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Qualification { get; set; }
        public List<Leave> Leaves { get; set; } = new List<Leave>();
        public int TotalLeaveDays { get; set; }

}