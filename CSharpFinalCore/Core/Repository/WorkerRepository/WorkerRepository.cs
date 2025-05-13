using CSharpFinalCore.Core.Entity;

namespace CSharpFinalCore.Core.Repository.WorkerRepository;

public abstract class WorkerRepository
{
    public abstract Task<bool> UpdateTaskWorker(int taskId, string taskStatus);
    public abstract Task<List<Tasks>?> GetAllTasksByEmployeeId(int employeeId);
    public abstract Task<Employees?> GetEmployeeInfoById(int employeeId);
    public abstract Task<Employees?> GetEmployeeInfoByEmail(string email);
}