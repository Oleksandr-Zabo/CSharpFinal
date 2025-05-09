namespace CSharpFinalData.Data.Models;

public class UserModel
{
    public int ? Id { get; set; }
    public string ? Login { get; set; }
    public string ? Password { get; set; }
    
    public UserModel(int id, string login, string password)
    {
        this.Login = login;
        this.Password = password;
        this.Id = id;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Login: {Login}, Password: {Password}";
    }
}