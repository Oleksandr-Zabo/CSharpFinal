using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CSharpFinalData.Data.Models;

[Table("Employees")]
public class EmployeesModel: BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    public string Id { get; set; } 
    
    [Column("Name")]
    public string Name { get; set; }
    
    [Column("Email")]
    public string Email { get; set; }
    
    [Column("RoleId")]
    public int RoleId { get; set; }
    
    [Column("Password")]
    public string Password { get; set; }
    
    public EmployeesModel()
    {
        Id = string.Empty; 
        Name = string.Empty;
        Email = string.Empty;
        RoleId = 0;
        Password = string.Empty;
    }
    
    public EmployeesModel(string id, string name, string email, int roleId, string password): this()
    {
        Id = id;
        Name = name;
        Email = email;
        RoleId = roleId;
        Password = password;
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Email: {Email}, RoleId: {RoleId}";
    }
}