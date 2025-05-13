using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CSharpFinalData.Data.Models;

[Table("Roles")]
public class RolesModel: BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    
    [Column("RoleName")]
    public string RoleName { get; set; }
    
    public RolesModel()
    {
        Id = 0;
        RoleName = string.Empty;
    }
    
    public RolesModel(int id, string roleName): this()
    {
        Id = id;
        RoleName = roleName;
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, RoleName: {RoleName}";
    }
    
}