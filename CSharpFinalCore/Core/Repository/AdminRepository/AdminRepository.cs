using CSharpFinalCore.Core.Entity;

namespace CSharpFinalCore.Core.Repository.AdminRepository;

public abstract class AdminRepository
{
    public abstract Task<List<Roles>> GetAllRolesAsync();
    public abstract Task<bool> IsUserByEmailAsync(string email);
    public abstract Task<List<Employees>> GetAllEmployeesAsync();
    public abstract Task<bool> DeleteEmployeeAsync(string id);
    public abstract Task<bool> AddEmployeeAsync(Employees employee);
    
    
}