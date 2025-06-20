using CSharpFinalCore.Core.Entity;
using CSharpFinalCore.Core.Repository.AdminRepository;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinalData.Data.RepositoryImpl.AdminRepositoryImpl;

public class AdminRepositoryImpl : AdminRepository
{
    private readonly SupabaseService _supabaseService;

    public AdminRepositoryImpl(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService ?? throw new ArgumentNullException(nameof(supabaseService));
    }
    

    public override async Task<List<Roles>> GetAllRolesAsync()
    {
        if (_supabaseService == null)
        {
            return new List<Roles>();
        }
    
        var rolesModels = await _supabaseService.GetAllRolesAsync() ?? new List<RolesModel>();
        return rolesModels.Select(rm => new Roles(rm.Id, rm.RoleName)).ToList();
    }
    
    public override async Task<List<Employees>> GetAllEmployeesAsync()
    {
        if (_supabaseService == null)
        {
            return new List<Employees>();
        }
    
        var employeesModels = await _supabaseService.GetAllEmployeesAsync() ?? new List<EmployeesModel>();
        return employeesModels.Select(em => new Employees(em.Id, em.Name, em.Email, em.RoleId, em.Password)).ToList();
    }

    public override async Task<bool> DeleteEmployeeAsync(string id)
    {
        if (_supabaseService == null)
        {
            return false;
        }
        return await _supabaseService.DeleteEmployeeAsync(id)!;
    }

    public override async Task<bool> AddEmployeeAsync(Employees employee)
    {
        var employeeModel = new AdapterEmployeeToModel(employee);
        
        return await (_supabaseService != null ? 
            Task.FromResult((await _supabaseService.RegisterAsync(employeeModel)) != null) : 
            Task.FromResult(false));
    }

    public override async Task<bool> IsUserByEmailAsync(string email)
    {
        if (_supabaseService == null)
        {
            return false;
        }
    
        return await _supabaseService.IsEmployeeByEmail(email);
    }

    public async Task Logout()
    {
        if (_supabaseService == null)
        {
            return;
        }
        try
        {
            await _supabaseService.LogoutAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout() raised an exception: {ex.Message}");
            throw;
        }
    }
}