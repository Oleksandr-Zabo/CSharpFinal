using CSharpFinalCore.Core.Entity;
using CSharpFinalCore.Core.Repository.WorkerRepository;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinalData.Data.RepositoryImpl.WorkerRepositoryImpl;

public class WorkerRepositoryImpl: WorkerRepository
{
    private readonly SupabaseService _supabaseService;

    public WorkerRepositoryImpl(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService ?? throw new ArgumentNullException(nameof(supabaseService), "SupabaseService cannot be null");
    }

    public override async Task<bool> UpdateTaskWorker(int taskId, string taskStatus)
    {
        try
        {
            await _supabaseService.InitServiceAsync();
            return await _supabaseService.UpdateTaskWorker(taskId, taskStatus);
        }
        catch (Exception ex)
        {
            throw new Exception($"UpdateTaskWorker(int taskId, string taskStatus) failed: {ex.Message}");
        }
    }

    public override async Task<List<Tasks>?> GetAllTasksByEmployeeId(int employeeId)
    {
        try
        {
            await _supabaseService.InitServiceAsync();
            var tasks = await _supabaseService.GetAllTasksByEmployeeId(employeeId);
            return tasks?.Select(t => new AdapterTaskFromModel(t)).Cast<Tasks>().ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"GetAllTasksByEmployeeId(int employeeId) failed: {ex.Message}");
        }
    }

    public override async Task<Employees?> GetEmployeeInfoById(int employeeId)
    {
        try
        {
            await _supabaseService.InitServiceAsync();
            var employeeModel = await _supabaseService.GetEmployeeInfoById(employeeId);
            if (employeeModel == null) return null;

            return new AdapterEmployeeFromModel(employeeModel);
        }
        catch (Exception ex)
        {
            throw new Exception($"GetEmployeeInfoById(int employeeId) failed: {ex.Message}");
        }
    }

    public override async Task<Employees?> GetEmployeeInfoByEmail(string email)
    {
        try
        {
            await _supabaseService.InitServiceAsync();
            var employeeModel = await _supabaseService.GetEmployeeInfoByEmail(email);
            if (employeeModel == null) return null;

            return new AdapterEmployeeFromModel(employeeModel);
        }
        catch (Exception ex)
        {
            throw new Exception($"GetEmployeeInfoByEmail(string email) failed: {ex.Message}");
        }
    }

    public async Task<List<RolesModel>?> GetAllRolesAsync()
    {
        try
        {
            await _supabaseService.InitServiceAsync();
            var roles = await _supabaseService.GetAllRolesAsync();
            return roles?.Select(r => new RolesModel { Id = r.Id, RoleName = r.RoleName }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"GetAllRolesAsync() failed: {ex.Message}");
        }
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
            throw new Exception($"Logout() raised an exception: {ex.Message}");
        }
    }
}