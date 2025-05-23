using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinal.Pages;

public partial class ManagerPage : UserControl
{
    private readonly SupabaseService _supabaseService;
    private readonly Employees _employee;
    private List<Employees> _workers = new();
    private List<TaskViewModel> _tasks = new();

    public ManagerPage(Employees employee, SupabaseService? service)
    {
        InitializeComponent();
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
        _supabaseService = service ?? throw new ArgumentNullException(nameof(service));
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
            var allEmployees = await _supabaseService.GetAllEmployeesAsync();
            _workers = allEmployees
                .Where(w => w.RoleId == 2 || w.RoleId == 3)
                .Select(w => new Employees
                {
                    Id = w.Id,
                    Name = w.Name,
                    RoleId = w.RoleId,
                    // Map other properties as needed
                })
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
            var allTasks = await _supabaseService.GetAllTasksAsync();
            // Map tasks to view model with worker name
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
            await _supabaseService.CreateTaskAsync(selectedWorker.Id, description, deadline, "new");
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
        _ = _supabaseService.Logout();
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        mainWindow?.MainWindowFrame.Navigate(new LoginPage());
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