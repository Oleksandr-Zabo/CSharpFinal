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
    private readonly Employees _employee;
    private List<Employees> _workers = new();
    private List<TaskViewModel> _tasks = new();

    public ManagerPage(Employees employee, ManagerRepositoryImpl? repository)
    {
        InitializeComponent();
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
        _managerRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        Loaded += ManagerPage_Loaded;
    }

    private async void ManagerPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadWorkersAsync();
        await LoadTasksAsync();
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
                WorkerId = t.EmployeeId,
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

    private async void OnUpdateTasksClick(object sender, RoutedEventArgs e)
    {
        await LoadTasksAsync();
    }

    private void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете вийти?", "Підтвердження", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;
        _ = _managerRepository.Logout();
        var mainWindow = (MainWindow)Application.Current.MainWindow;
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

    // View model for DataGrid
    private class TaskViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
    }
}
