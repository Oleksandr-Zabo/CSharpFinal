using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinal.Pages;

public partial class WorkerPage : UserControl
{
    private readonly SupabaseService _supabaseService;
    private readonly Employees _employee;
    private List<TaskViewModel> _tasks = new();
    private string _roleName = "";

    public WorkerPage(Employees employee, SupabaseService? service)
    {
        InitializeComponent();
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
        _supabaseService = service ?? throw new ArgumentNullException(nameof(service));
        Loaded += WorkerPage_Loaded;
    }

    private async void WorkerPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadWorkerInfoAsync();
        await LoadTasksAsync();
    }

    private async Task LoadWorkerInfoAsync()
    {
        WorkerNameText.Text = _employee.Name;
        WorkerEmailText.Text = _employee.Email;
        _roleName = await GetRoleNameAsync(_employee.RoleId);
        WorkerRoleText.Text = $"Роль: {_roleName}";
    }

    private async Task<string> GetRoleNameAsync(int roleId)
    {
        try
        {
            var roles = await _supabaseService.GetAllRolesAsync();
            var role = roles?.FirstOrDefault(r => r.Id == roleId);
            return role?.RoleName ?? "Невідомо";
        }
        catch
        {
            return "Невідомо";
        }
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            var allTasks = await _supabaseService.GetAllTasksByEmployeeId(_employee.Id);
            _tasks = allTasks?.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Description = t.Description,
                Deadline = t.Deadline,
                Status = NormalizeStatus(t.Status)
            }).ToList() ?? new List<TaskViewModel>();
            TasksDataGrid.ItemsSource = _tasks;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не вдалося завантажити завдання: " + ex.Message);
        }
    }

    private string NormalizeStatus(string status)
    {
        return status switch
        {
            "New" or "InProgress" or "Finished" => status,
            _ => "New"
        };
    }

    private async void OnNextPhaseClick(object sender, RoutedEventArgs e)
    {
        if (TasksDataGrid.SelectedItem is not TaskViewModel selectedTask)
        {
            MessageBox.Show("Оберіть завдання для оновлення.");
            return;
        }

        string nextStatus = selectedTask.Status switch
        {
            "New" => "InProgress",
            "InProgress" => "Finished",
            "Finished" => "Finished",
            _ => "New"
        };

        if (selectedTask.Status == "Finished")
        {
            MessageBox.Show("Завдання уже виконано.");
            return;
        }

        try
        {
            await _supabaseService.UpdateTaskWorker(selectedTask.Id, nextStatus);
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при оновленні завдання: " + ex.Message);
        }
    }

    private void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете вийти?", "Підтвердження", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;
        _ = _supabaseService.Logout();
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        mainWindow?.MainWindowFrame.Navigate(new LoginPage());
    }

    private class TaskViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
    }
}