namespace CSharpFinalCore.Core.Entity;

public class Employees
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
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