using CSharpFinalCore.Core.Entity;
using CSharpFinalCore.Core.Repository.AdminRepository;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinalData.Data.RepositoryImpl.AdminRepositoryImpl;

public class AdminRepositoryImpl: AdminRepository
{
    private readonly SupabaseService? _supabaseService;
    
    public AdminRepositoryImpl(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
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

    public override async Task<bool> DeleteEmployeeAsync(int id)
    {
        if (_supabaseService == null)
        {
            return false;
        }
    
        return await _supabaseService.DeleteEmployeeAsync(id)!;
    }

    public override async Task<bool> AddEmployeeAsync(Employees employee)
    {
        var employeeModel = new EmployeesModel
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Password = employee.Password,
            RoleId = employee.RoleId
        };
        
        return await (_supabaseService != null ? 
            Task.FromResult((await _supabaseService.RegisterAsync(employeeModel)) != null) : 
            Task.FromResult(false));
    }

    
}