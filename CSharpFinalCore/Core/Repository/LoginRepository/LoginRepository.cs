using CSharpFinalCore.Core.Entity;

namespace CSharpFinalCore.Core.Repository.LoginRepository;

public abstract class LoginRepository<T>
{
    public abstract Task<T> LoginAsync(string email, string password);
    
    //get employee by user
    public abstract Task<Employees> GetEmployeeByUserAsync(string email, string password);
}