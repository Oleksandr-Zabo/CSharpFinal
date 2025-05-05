using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CSharpFinalCore.Core.Models;

[Table("Roles")]
public class Roles
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    
    [Column("RoleName")]
    public string RoleName { get; set; }
    
    public Roles()
    {
        Id = 0;
        RoleName = string.Empty;
    }
    
    public Roles(int id, string roleName)
    {
        Id = id;
        RoleName = roleName;
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, RoleName: {RoleName}";
    }
    
}