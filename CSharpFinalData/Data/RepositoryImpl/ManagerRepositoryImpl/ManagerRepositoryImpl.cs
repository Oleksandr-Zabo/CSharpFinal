using CSharpFinalCore.Core.Entity;
using CSharpFinalCore.Core.Repository.ManagerRepository;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinalData.Data.RepositoryImpl.ManagerRepositoryImpl;

public class ManagerRepositoryImpl(SupabaseService supabaseService) : ManagerRepository
{
    private readonly SupabaseService? _supabaseService = supabaseService;

    public override async Task<List<Employees>> GetAllEmployeesAsync()
    {
        if (_supabaseService == null)
        {
            return new List<Employees>();
        }
    
        var employeesModels = await _supabaseService.GetAllEmployeesAsync() ?? new List<EmployeesModel>();
        return employeesModels.Select(em => new Employees(em.Id, em.Name, em.Email, em.RoleId, em.Password)).ToList();
    }

    public override async Task<bool> CreateTaskAsync(int taskEmployeeId, string taskDescription, DateTime taskDeadLine, string taskStatus)
    {
        if (_supabaseService == null)
        {
            return false;
        }
    
        try
        {
            return await _supabaseService.CreateTaskAsync(taskEmployeeId, taskDescription, taskDeadLine, taskStatus);
        }
        catch (Exception ex)
        {
            throw new Exception($"CreateTaskAsync(int taskEmployeeId, string taskDescription, DateTime taskDeadLine, string taskStatus) raised an exception: {ex.Message}");
        }
    }

    public override async Task<List<Tasks>?> GetAllTasksAsync()
    {
        if (_supabaseService == null)
        {
            return new List<Tasks>();
        }
    
        try
        {
            var tasksModels = await _supabaseService.GetAllTasksAsync() ?? new List<TasksModel>();
            return tasksModels.Select(tm => (Tasks)new AdapterTaskFromModel(tm)).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"GetAllTasksAsync() raised an exception: {ex.Message}");
        }
    }

    public override Task<bool> DeleteAllFinishedTasksAsync()
    {
        if (_supabaseService == null)
        {
            return Task.FromResult(false);
        }
    
        try
        {
            return _supabaseService.DeleteAllFinishedTasksAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"DeleteAllFinishedTasksAsync() raised an exception: {ex.Message}");
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