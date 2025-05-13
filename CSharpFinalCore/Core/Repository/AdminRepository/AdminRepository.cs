using CSharpFinalCore.Core.Entity;

namespace CSharpFinalCore.Core.Repository.AdminRepository;

public abstract class AdminRepository
{
    public abstract Task<List<Roles>> GetAllRolesAsync();
    public abstract Task<List<Employees>> GetAllEmployeesAsync();
    public abstract Task<bool> DeleteEmployeeAsync(int id);
    public abstract Task<bool> AddEmployeeAsync(Employees employee);
    
    
}