namespace CSharpFinalCore.Core.Entity;

public class Roles
{
    public int Id { get; set; }
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