using CSharpFinalCore.Core.Entity;

namespace CSharpFinalCore.Core.Repository.AdminRepository;

public abstract class AdminRepository<T>
{
    public abstract Task<List<Roles>> GetAllRolesAsync();
    public abstract Task<bool> DeleteAsync(int id);
    public abstract Task<bool> AddEmployeeAsync(Employees employee);
    public abstract Task<bool> DeleteEmployeeAsync(int id);
    
}