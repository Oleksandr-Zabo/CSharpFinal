namespace CSharpFinalCore.Core.Entity;

public class Tasks
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string EmployeeId { get; set; } 
    public DateTime Deadline { get; set; }
    public string Status { get; set; }

    public Tasks()
    {
        Id = 0;
        Description = string.Empty;
        EmployeeId = string.Empty; 
        Deadline = DateTime.Now;
        Status = string.Empty;
    }

    public Tasks(int id, string description, string employeeId, DateTime deadline, string status)
    {
        Id = id;
        Description = description;
        EmployeeId = employeeId;
        Deadline = deadline;
        Status = status;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Description: {Description}, EmployeeId: {EmployeeId}, DueDate: {Deadline}, Status: {Status}";
    }
    
}