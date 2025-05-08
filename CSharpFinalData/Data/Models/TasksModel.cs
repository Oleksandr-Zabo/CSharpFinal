using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CSharpFinalData.Data.Models;

[Table("Tasks")]
public class TasksModel: BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    
    [Column ("EmployeeId")]
    public int EmployeeId { get; set; }
    
    [Column("Description")]
    public string Description { get; set; }
    
    [Column("Deadline")]
    public DateTime Deadline { get; set; }
    
    [Column("Status")]
    public string Status { get; set; }
    
    public TasksModel()
    {
        Id = 0;
        EmployeeId = 0;
        Description = string.Empty;
        Deadline = DateTime.Now;
        Status = string.Empty;
    }
    
    public TasksModel(int id, int employeeId, string description, DateTime deadline, string status)
    {
        Id = id;
        EmployeeId = employeeId;
        Description = description;
        Deadline = deadline;
        Status = status;
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, EmployeeId: {EmployeeId}, Description: {Description}, Deadline: {Deadline}, Status: {Status}";
    }
    
}