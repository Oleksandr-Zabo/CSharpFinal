using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.Models;
using Supabase.Gotrue;
using Client = Supabase.Client;
using Supabase.Postgrest;
using static Supabase.Postgrest.Constants;

namespace CSharpFinalData.Data.Source.Remote.SupabaseDB;


public class SupabaseService
{
    
    private const string SupabaseUrl = "https://blsbwhilzmlhlhxywfpl.supabase.co";
    private const string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJsc2J3aGlsem1saGxoeHl3ZnBsIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDU5NDg0MTYsImV4cCI6MjA2MTUyNDQxNn0.QN24DqtBr6wFqHNyAYw-XOoHGxbxx0fneOoDJxFsDHo";
    
    private readonly Supabase.Client _client;
        
    public User? SupabaseUser { get; set; } = null;
        
    public bool IsLoggedIn { get; set; } = false;
        
    public SupabaseService()
    {
        try
        {
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
            };
            _client = new Client(SupabaseUrl, SupabaseKey, options);
        }
        catch (Exception ex)
        {
            throw new Exception($"SupabaseService() raise Exception: {ex.Message}");
        }
    }
        
    public async Task InitServiceAsync()
    {
        try
        {
            await _client.InitializeAsync()!;
        }
        catch (Exception ex)
        {
            throw new Exception($"InitServiceAsync() raise Exception: {ex.Message}");
        }
    }


    public void SetAuthUser()
    {
        if (_client.Auth.CurrentUser != null)
        {
            SupabaseUser = _client.Auth.CurrentUser;
            IsLoggedIn = true;
        }
    }
    
    public async Task<Session?> LoginAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignIn(email, password);
            if (session != null)
            {
                SupabaseUser = session.User;
                IsLoggedIn = true;
                return session;
            }
            else
            {
                throw new Exception("Login failed.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Login(string email, string password) raise Exception: {ex.Message}");
        }
    }
            
    public async Task Logout()
    {
        try
        {
            await _client.Auth.SignOut();
            IsLoggedIn = false;
        }
        catch (Exception ex)
        {
            throw new Exception($"Logout() raise Exception: {ex.Message}");
        }
    }
    
    // get employee by user
    public async Task<EmployeesModel?> GetEmployeeByUserAsync(string email, string password)
    {
        try
        {
            var result = await _client.From<EmployeesModel>().Get();
            var employee = result.Models.FirstOrDefault(e => e.Email == email && e.Password == password);
            return employee;
        }
        catch (Exception ex)
        {
            throw new Exception($"GetEmployeeByUser(string email, string password) raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Admin - is employee by email
    public async Task<bool> IsEmployeeByEmail(string email)
    {
        try
        {
            var result = await _client.From<EmployeesModel>().Get();
            var employee = result.Models.FirstOrDefault(e => e.Email == email);
            return employee != null;
        }
        catch (Exception ex)
        {
            throw new Exception($"IsEmployeeByEmail(string email) raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Admin - register new employee
    public async Task<Session?> RegisterAsync(EmployeesModel employee)
    {
        try
        {
            // Check if the user already exists by attempting to log in
            var existingSession = await _client.Auth.SignIn(employee.Email, employee.Password);
            if (existingSession != null)
            {
                throw new Exception("User already exists in the system.");
            }
        }
        catch (Exception ex)
        {
            // Ignore the login failure and proceed with registration
            if (!ex.Message.Contains("Invalid login credentials"))
            {
                throw new Exception($"Error during user existence check: {ex.Message}");
            }
        }
    
        try
        {
            // Register the user- register+ add data to tables
            var session = await _client.Auth.SignUp(employee.Email, employee.Password);
            if (session != null)
            {
                // Add the employee to the Employees table
                var result = await _client.From<EmployeesModel>().Insert(new EmployeesModel
                {
                    Name = employee.Name,
                    Email = employee.Email,
                    RoleId = employee.RoleId,
                    Password = employee.Password
                });
    
                if (result.Models.Count > 0)
                {
                    return session;
                }
                else
                {
                    throw new Exception("Failed to add employee to the database.");
                }
            }
            else
            {
                throw new Exception("Registration failed.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"RegisterAsync(EmployeesModel employee) raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Admin - get all roles
    public async Task<List<RolesModel>?> GetAllRolesAsync()
    {
        try
        {
            var result = await _client.From<RolesModel>().Get();
            return result.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"GetAllRoles() raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Admin - get all employees
    public async Task<List<EmployeesModel>?> GetAllEmployeesAsync()
    {
        try
        {
            var result = await _client.From<EmployeesModel>().Get();
            return result.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"GetAllEmployees() raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Admin - delete employee and check if no admin
    public async Task<bool>? DeleteEmployeeAsync(int employeeId)
    {
        try
        {
            // Fetch the employee to check their role
            var employee = await _client
                .From<EmployeesModel>()
                .Where(e => e.Id == employeeId)
                .Single();
    
            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }
    
            // Check if the employee is an admin
            var role = await _client
                .From<RolesModel>()
                .Where(r => r.Id == employee.RoleId)
                .Single();
    
            if (role != null && role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot delete an admin employee.");
            }
            // Proceed with deletion
            await _client
                .From<EmployeesModel>()
                .Where(e => e.Id == employeeId)
                .Delete();

            return true; // Assuming the deletion was successful if no exception was thrown
        }
        catch (Exception ex)
        {
            throw new Exception($"DeleteEmployee(int employeeId) raise Exception: {ex.Message}");
        }
    }
    
    
    // for Role: Manager - create a task
    
    public async Task<bool> CreateTaskAsync(int taskEmployeeId, string taskDescription, DateTime taskDeadLine, string taskStatus)
    {
        try
        {
            var result = await _client.From<TasksModel>().Insert(new TasksModel
            {
                EmployeeId = taskEmployeeId,
                Description = taskDescription,
                Deadline = taskDeadLine,
                Status = taskStatus
            });
    
            return result.Models.Count > 0; // Success if at least one record is returned
        }
        catch (Exception ex)
        {
            throw new Exception($"CreateTask(int taskEmployeeId, string taskDescription, DateTime taskDeadLine, string taskStatus) raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Manager - get all tasks
    public async Task<List<TasksModel>?> GetAllTasksAsync()
    {
        try
        {
            var result = await _client.From<TasksModel>().Get();
            return result.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"GetAllTasks() raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Worker - get all tasks by employeeId
    public async Task<List<TasksModel>?> GetAllTasksByEmployeeId(int employeeId)
    {
        try
        {
            var result = await _client.From<TasksModel>().Get();
            var tasks = result.Models.Where(task => task.EmployeeId == employeeId).ToList();
            return tasks;
        }
        catch (Exception ex)
        {
            throw new Exception($"GetAllTasksByEmployeeId(int employeeId) raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Worker- update task
    public async Task<bool> UpdateTaskWorker(int taskId, string taskStatus)
    {
        try
        {
            var result = await _client
                .From<TasksModel>()
                .Where(x => x.Id == taskId)
                .Update(new TasksModel { Status = taskStatus });
    
            return result.Models.Count > 0; // Success if at least one record is updated
        }
        catch (Exception ex)
        {
            throw new Exception($"UpdateTaskWorker(int taskId, string taskStatus) raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Worker - get info about worker by id
    public async Task<EmployeesModel?> GetEmployeeInfoById(int employeeId)
    {
        try
        {
            var result = await _client.From<EmployeesModel>().Get();
            var employee = result.Models.FirstOrDefault(e => e.Id == employeeId);
            return employee;
        }
        catch (Exception ex)
        {
            throw new Exception($"GetEmployeeInfo(int employeeId) raise Exception: {ex.Message}");
        }
    }
    
    // for Role: Worker - get info about worker by email
    public async Task<EmployeesModel?> GetEmployeeInfoByEmail(string email)
    {
        try
        {
            var result = await _client.From<EmployeesModel>().Get();
            var employee = result.Models.FirstOrDefault(e => e.Email == email);
            return employee;
        }
        catch (Exception ex)
        {
            throw new Exception($"GetEmployeeInfo(string email) raise Exception: {ex.Message}");
        }
    }
    
}