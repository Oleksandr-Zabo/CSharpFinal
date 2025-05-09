using CSharpFinalCore.Core.Entity;

namespace CSharpFinalCore.Core.Repository.ManagerRepository;

public abstract class ManagerRepository
{
    public abstract Task<List<Employees>> GetAllEmployeesAsync();
    public abstract Task<bool> CreateTaskAsync(int taskEmployeeId, string taskDescription, DateTime taskDeadLine, string taskStatus);
    public abstract Task<List<Tasks>?> GetAllTasksAsync();

}