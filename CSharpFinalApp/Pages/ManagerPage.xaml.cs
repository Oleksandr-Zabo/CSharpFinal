using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.RepositoryImpl.ManagerRepositoryImpl;
using System.IO;

namespace CSharpFinal.Pages;

public partial class ManagerPage : UserControl
{
    private readonly ManagerRepositoryImpl _managerRepository;
    private readonly Employees? _employee;
    private List<Employees> _workers = new();
    private List<TaskViewModel> _tasks = new();
    private System.Timers.Timer? _workerMonitorTimer;
    private System.Timers.Timer? _taskMonitorTimer;
    private List<Employees> _currentWorkers = new(); // For monitoring changes
    private List<TaskViewModel> _currentTasks = new(); // For monitoring changes

    public ManagerPage(Employees employee, ManagerRepositoryImpl? repository)
    {
        InitializeComponent();
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
        _managerRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        Loaded += ManagerPage_Loaded;
        Unloaded += ManagerPage_Unloaded;
        // Timer for monitoring workers every 10 seconds
        _workerMonitorTimer = new System.Timers.Timer(WORKER_MONITOR_INTERVAL);
        _workerMonitorTimer.Elapsed += async (s, e) => await MonitorWorkersAsync();
        _workerMonitorTimer.AutoReset = true;
        _workerMonitorTimer.Enabled = true;
        // Timer for monitoring tasks every 3 seconds
        _taskMonitorTimer = new System.Timers.Timer(TASK_MONITOR_INTERVAL);
        _taskMonitorTimer.Elapsed += async (s, e) => await MonitorTasksAsync();
        _taskMonitorTimer.AutoReset = true;
        _taskMonitorTimer.Enabled = true;
    }

    private async void ManagerPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadWorkersAsync();
        _currentWorkers = GetCurrentWorkersSnapshot();
        await LoadTasksAsync();
        _currentTasks = GetCurrentTasksSnapshot();
        _workerMonitorTimer?.Start();
        _taskMonitorTimer?.Start();
    }

    private async Task LoadWorkersAsync()
    {
        try
        {
           var allEmployees = await _managerRepository.GetAllEmployeesAsync();
           _workers = allEmployees
               .Where(w => w.RoleId == 2 || w.RoleId == 3)
               .ToList();
           WorkerComboBox.ItemsSource = _workers;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не вдалося завантажити працівників: " + ex.Message);
        }
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            var allTasks = await _managerRepository.GetAllTasksAsync();
            _tasks = allTasks.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Description = t.Description,
                WorkerId = t.EmployeeId, // Updated to string
                WorkerName = _workers.FirstOrDefault(w => w.Id == t.EmployeeId)?.Name ?? "—",
                Deadline = t.Deadline,
                Status = t.Status
            }).ToList();
            TasksDataGrid.ItemsSource = _tasks;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не вдалося завантажити завдання: " + ex.Message);
        }
    }

    private async void OnAddTaskClick(object sender, RoutedEventArgs e)
    {
        if (WorkerComboBox.SelectedItem is not Employees selectedWorker)
        {
            MessageBox.Show("Оберіть працівника для завдання.");
            return;
        }
        var description = TaskDescriptionTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(description))
        {
            MessageBox.Show("Введіть опис завдання.");
            return;
        }
        if (DeadlineDatePicker.SelectedDate is not DateTime deadline)
        {
            MessageBox.Show("Оберіть дату дедлайну.");
            return;
        }
        if (deadline <= DateTime.Now)
        {
            MessageBox.Show("Дата дедлайну повинна бути пізніше поточної дати.");
            return;
        }

        try
        {
            await _managerRepository.CreateTaskAsync(selectedWorker.Id, description, deadline, "New");
            MessageBox.Show("Завдання додано!");
            TaskDescriptionTextBox.Text = "";
            DeadlineDatePicker.SelectedDate = null;
            WorkerComboBox.SelectedItem = null;
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при додаванні завдання: " + ex.Message);
        }
    }

    private async void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете вийти?", "Підтвердження", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;
        try
        {
            await _managerRepository.Logout();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при виході: " + ex.Message);
            return;
        }
        var mainWindow = Application.Current?.MainWindow as MainWindow;
        mainWindow?.MainWindowFrame.Navigate(new LoginPage());
    }
    
    private async void OnCreateReportClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var reportLines = new List<string>
            {
                $"Звіт створено: {DateTime.Now:G}",
                "Список завдань:"
            };
    
            foreach (var task in _tasks)
            {
                reportLines.Add($"Опис: {task.Description}");
                reportLines.Add($"Працівник: {task.WorkerName}");
                reportLines.Add($"Дедлайн: {task.Deadline:d}");
                reportLines.Add($"Статус: {task.Status}");
                reportLines.Add(""); // Empty line between tasks
            }
    
            reportLines.Add("-------------------------");
            reportLines.Add(""); // Extra line for separation
    
            // Get a project directory
            var projectDir = AppDomain.CurrentDomain.BaseDirectory;
            var reportDir = Path.Combine(projectDir, "manager's_report");
            Directory.CreateDirectory(reportDir);
    
            var filePath = Path.Combine(reportDir, "manager_report.txt");
    
            await File.AppendAllLinesAsync(filePath, reportLines);
    
            MessageBox.Show($"Звіт додано до файлу:\n{filePath}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при створенні звіту: " + ex.Message);
        }
    }

    private async void OnDeleteFinishedTasksClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете видалити всі виконані завдання?", "Підтвердження", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;
        try
        {
            var success = await _managerRepository.DeleteAllFinishedTasksAsync();
            if (success)
            {
                MessageBox.Show("Всі виконані завдання видалено.");
                await LoadTasksAsync();
            }
            else
            {
                MessageBox.Show("Не вдалося видалити виконані завдання.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при видаленні виконаних завдань: " + ex.Message);
        }
    }

    private void ManagerPage_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_workerMonitorTimer != null)
        {
            _workerMonitorTimer.Stop();
            _workerMonitorTimer.Dispose();
            _workerMonitorTimer = null;
        }
        if (_taskMonitorTimer != null)
        {
            _taskMonitorTimer.Stop();
            _taskMonitorTimer.Dispose();
            _taskMonitorTimer = null;
        }
    }

    private List<Employees> GetCurrentWorkersSnapshot()
    {
        // Return a shallow copy for comparison
        return _workers.Select(w => w).ToList();
    }

    private async Task MonitorWorkersAsync()
    {
        try
        {
            var allEmployees = await _managerRepository.GetAllEmployeesAsync();
            var filteredWorkers = allEmployees.Where(w => w.RoleId == 2 || w.RoleId == 3).ToList();
            // Compare with _currentWorkers
            if (!AreWorkersEqual(filteredWorkers, _currentWorkers))
            {
                _workers = filteredWorkers;
                _currentWorkers = filteredWorkers.Select(w => w).ToList();
                await Dispatcher.InvokeAsync(() => WorkerComboBox.ItemsSource = _workers);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не вдалося оновити список працівників: " + ex.Message);
        }
    }

    private bool AreWorkersEqual(List<Employees> list1, List<Employees> list2)
    {
        if (list1.Count != list2.Count) return false;
        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i].Id != list2[i].Id || list1[i].Name != list2[i].Name)
                return false;
        }
        return true;
    }

    private List<TaskViewModel> GetCurrentTasksSnapshot()
    {
        // Convert TaskViewModel objects to Task objects before creating the snapshot
        var tasks = ConvertTaskViewModelsToTasks(_tasks);
        return CreateTaskViewModelList(tasks, null);
    }

    private List<Task> ConvertTaskViewModelsToTasks(List<TaskViewModel> taskViewModels)
    {
        return taskViewModels.Select(tv => new Task
        {
            Id = tv.Id,
            Description = tv.Description,
            WorkerId = tv.WorkerId,
            WorkerName = tv.WorkerName,
            Deadline = tv.Deadline,
            Status = tv.Status
        }).ToList();
    }
    private List<TaskViewModel> CreateTaskViewModelList(List<Task> tasks, List<Employees>? workers)
    {
        return tasks.Select(t => new TaskViewModel
        {
            Id = t.Id,
            Description = t.Description,
            WorkerId = t.WorkerId,
            WorkerName = workers?.FirstOrDefault(w => w.Id == t.WorkerId)?.Name ?? t.WorkerName ?? "—",
            Deadline = t.Deadline,
            Status = t.Status
        }).ToList();
    }

    private async Task MonitorTasksAsync()
    {
        try
        {
            var allTasks = await _managerRepository.GetAllTasksAsync();
            var newTasks = allTasks.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Description = t.Description,
                WorkerId = t.EmployeeId,
                WorkerName = _workers.FirstOrDefault(w => w.Id == t.EmployeeId)?.Name ?? "—",
                Deadline = t.Deadline,
                Status = t.Status
            }).ToList();
            if (!AreTasksEqual(newTasks, _currentTasks))
            {
                _tasks = newTasks;
                _currentTasks = GetCurrentTasksSnapshot();
                await Dispatcher.InvokeAsync(() => TasksDataGrid.ItemsSource = _tasks);
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show("Помилка при моніторингу завдань: " + ex.Message);
        }
    }

    private bool AreTasksEqual(List<TaskViewModel> list1, List<TaskViewModel> list2)
    {
        var sortedList1 = list1.OrderBy(t => t.Id).ToList();
        var sortedList2 = list2.OrderBy(t => t.Id).ToList();

        if (sortedList1.Count != sortedList2.Count) return false;
        for (int i = 0; i < sortedList1.Count; i++)
        {
            if (sortedList1[i].Id != sortedList2[i].Id ||
                sortedList1[i].Description != sortedList2[i].Description ||
                sortedList1[i].WorkerId != sortedList2[i].WorkerId ||
                sortedList1[i].WorkerName != sortedList2[i].WorkerName ||
                sortedList1[i].Deadline != sortedList2[i].Deadline ||
                sortedList1[i].Status != sortedList2[i].Status)
                return false;
        }
        return true;
    }

    // View model for DataGrid
    private class TaskViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string WorkerId { get; set; } = string.Empty;
        public string WorkerName { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
