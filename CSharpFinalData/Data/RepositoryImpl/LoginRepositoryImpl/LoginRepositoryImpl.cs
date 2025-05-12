using CSharpFinalCore.Core.Entity;
using CSharpFinalCore.Core.Repository.LoginRepository;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinalData.Data.RepositoryImpl.LoginRepositoryImpl;

public class LoginRepositoryImpl: LoginRepository<UserModel>
{
    private readonly SupabaseService? _supabaseService;
    
    public LoginRepositoryImpl(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }
    
   public override async Task<UserModel> LoginAsync(string email, string password)
   {
       try
       {
           var session = await _supabaseService?.LoginAsync(email, password)!;
           if (session?.User?.Id != null)
           {
               return new UserModel(
                   id: session.User.Id.GetHashCode(), // Assuming `Id` is a string, convert it to an int hash
                   login: email,
                   password: password
               );
           }
           throw new Exception("Login failed. User session is null.");
       }
       catch (Exception ex)
       {
           throw new Exception($"LoginAsync(string email, string password) raised an exception: {ex.Message}");
       }
   }

    public override async Task<Employees> GetEmployeeByUserAsync(string email, string password)
    {
    
        try
        {
            var employee = await _supabaseService?.GetEmployeeByUserAsync(email, password)!;
            if (employee != null)
            {
                return new Employees(
                    id: employee.Id,
                    name: employee.Name,
                    email: employee.Email,
                    roleId: employee.RoleId,
                    password: employee.Password
                );
            }
            else
            {
                throw new Exception("Employee not found.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"GetEmployeeByUserAsync(string email, string password) raised an exception: {ex.Message}");
        }
    }
}