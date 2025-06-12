using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.RepositoryImpl.WorkerRepositoryImpl;

namespace CSharpFinal.Pages;

public partial class WorkerPage : UserControl
{
    private readonly WorkerRepositoryImpl _workerRepository;
    private readonly Employees _employee;
    private List<TaskViewModel> _tasks = new();
    private List<TaskViewModel> _currentTasks = new(); // For monitoring changes
    private string _roleName = "";
    private System.Timers.Timer? _taskMonitorTimer;

    public WorkerPage(Employees employee, WorkerRepositoryImpl? repository)
    {
        InitializeComponent();
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
        _workerRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        Loaded += WorkerPage_Loaded;
        Unloaded += WorkerPage_Unloaded;
        // Set up a timer to monitor for new tasks every 3 seconds
        _taskMonitorTimer = new System.Timers.Timer(3000); // 3 seconds
        _taskMonitorTimer.Elapsed += async (s, e) => await MonitorTasksAsync();
        _taskMonitorTimer.AutoReset = true;
        _taskMonitorTimer.Enabled = true;
    }

    private async void WorkerPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadWorkerInfoAsync();
        await LoadTasksAsync();
        _currentTasks = GetCurrentTasksSnapshot();
        _taskMonitorTimer?.Start();
    }

    private async Task LoadWorkerInfoAsync()
    {
        WorkerNameText.Text = _employee.Name;
        WorkerEmailText.Text = _employee.Email;
        _roleName = _employee.RoleId switch
        {
            2 => "Офіціант", // Waiter
            3 => "Шеф-повар", // Chef
            _ => "Працівник"
        };
        WorkerRoleText.Text = _roleName;
    }

    private async Task<string> GetRoleNameAsync(int roleId)
    {
        try
        {
            var roles = await _workerRepository.GetAllRolesAsync();
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
            var allTasks = await _workerRepository.GetAllTasksByEmployeeId(_employee.Id);
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
         var updateResult = await _workerRepository.UpdateTaskWorker(selectedTask.Id, nextStatus);
         if (updateResult)
         {
             MessageBox.Show($@"Завдання оновлено до статусу: {nextStatus}");
         }
         else
         {
             MessageBox.Show("Не вдалося оновити завдання.");
         }
         await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при оновленні завдання: " + ex.Message);
        }
    }

    private async void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете вийти?", "Підтвердження", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;
        try
        {
            await _workerRepository.Logout();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.MainWindowFrame.Navigate(new LoginPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при виході: " + ex.Message);
        }
    }

    private void WorkerPage_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_taskMonitorTimer != null)
        {
            _taskMonitorTimer.Stop();
            _taskMonitorTimer.Dispose();
            _taskMonitorTimer = null;
        }
    }

    private List<TaskViewModel> GetCurrentTasksSnapshot()
    {
        // Return a shallow copy for comparison
        return _tasks.Select(t => new TaskViewModel
        {
            Id = t.Id,
            Description = t.Description,
            Deadline = t.Deadline,
            Status = t.Status
        }).ToList();
    }

    private async Task MonitorTasksAsync()
    {
        try
        {
            var allTasks = await _workerRepository.GetAllTasksByEmployeeId(_employee.Id);
            var newTasks = allTasks?.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Description = t.Description,
                Deadline = t.Deadline,
                Status = NormalizeStatus(t.Status)
            }).ToList() ?? new List<TaskViewModel>();
            if (!AreTasksEqual(newTasks, _currentTasks))
            {
                _tasks = newTasks;
                _currentTasks = GetCurrentTasksSnapshot();
                await Dispatcher.InvokeAsync(() => TasksDataGrid.ItemsSource = _tasks);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при моніторингу завдань: " + ex.Message);
        }
    }

    private bool AreTasksEqual(List<TaskViewModel> list1, List<TaskViewModel> list2)
    {
        if (list1.Count != list2.Count) return false;
        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i].Id != list2[i].Id ||
                list1[i].Description != list2[i].Description ||
                list1[i].Deadline != list2[i].Deadline ||
                list1[i].Status != list2[i].Status)
                return false;
        }
        return true;
    }

    private class TaskViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}