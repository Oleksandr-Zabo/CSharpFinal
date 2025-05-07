namespace CSharpFinalCore.Core.Entity;

using CSharpFinalData.Data.Models;

public class Tasks
{
    public int Id { get; set; }
    public string Description { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Deadline { get; set; }
    public string Status { get; set; }

    public Tasks()
    {
        Id = 0;
        Description = string.Empty;
        EmployeeId = 0;
        Deadline = DateTime.Now;
        Status = string.Empty;
    }

    //using pattern adapter
    public Tasks(TasksModel tasksModel)
    {
        Id = tasksModel.Id;
        Description = tasksModel.Description;
        EmployeeId = tasksModel.EmployeeId;
        Deadline = tasksModel.Deadline;
        Status = tasksModel.Status;
    }

    public Tasks(int id, string description, int employeeId, DateTime deadline, string status)
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