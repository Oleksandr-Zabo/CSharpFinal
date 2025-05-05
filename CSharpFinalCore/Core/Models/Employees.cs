using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CSharpFinalCore.Core.Models;

[Table("Employees")]
public class Employees
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    
    [Column("Name")]
    public string Name { get; set; }
    
    [Column("Email")]
    public string Email { get; set; }
    
    [Column("RoleId")]
    public int RoleId { get; set; }
    
    [Column("Password")]
    public string Password { get; set; }
    
    public Employees()
    {
        Id = 0;
        Name = string.Empty;
        Email = string.Empty;
        RoleId = 0;
        Password = string.Empty;
    }
    
    public Employees(int id, string name, string email, int roleId, string password)
    {
        Id = id;
        Name = name;
        Email = email;
        RoleId = roleId;
        Password = password;
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Email: {Email}, RoleId: {RoleId}, Password: {Password}";
    }
}