namespace CSharpFinalCore.Core.Repository.LoginRepository;

public abstract class LoginRepository<T>
{
    public abstract Task<T> LoginAsync(string email, string password);
}