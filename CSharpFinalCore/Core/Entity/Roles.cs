namespace CSharpFinalCore.Core.Entity;

using CSharpFinalData.Data.Models;

public class Roles
{
    public int Id { get; set; }
    public string RoleName { get; set; }

    public Roles()
    {
        Id = 0;
        RoleName = string.Empty;
    }
    
    //using pattern adapter
    public Roles(RolesModel rolesModel)
    {
        Id = rolesModel.Id;
        RoleName = rolesModel.RoleName;
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