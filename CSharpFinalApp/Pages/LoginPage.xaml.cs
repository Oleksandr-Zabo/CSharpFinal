using System.Windows;
using System.Windows.Controls;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.RepositoryImpl.LoginRepositoryImpl;
using CSharpFinalData.Data.RepositoryImpl.AdminRepositoryImpl;
using CSharpFinalData.Data.RepositoryImpl.ManagerRepositoryImpl;
using CSharpFinalData.Data.RepositoryImpl.WorkerRepositoryImpl;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinal.Pages;

public partial class LoginPage : UserControl
{
    private readonly SupabaseService _supabaseService;

    public LoginPage()
    {
        InitializeComponent();
        _supabaseService = new SupabaseService();// Initialize SupabaseService
    }

    private async void OnLoginButtonClick(object sender, RoutedEventArgs e)
    {
        var email = LoginTextBox.Text;
        var password = PasswordBox.Password;

        try
        {
            var loginRepository = new LoginRepositoryImpl(_supabaseService);
            var user = await loginRepository.LoginAsync(email, password);
            var employee = await loginRepository.GetEmployeeByUserAsync(email, password);

            MessageBox.Show("Вхід успішний!");
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            switch (employee.RoleId)
            {
                case 0:
                    // Admin
                    var adminRepo = new AdminRepositoryImpl(_supabaseService);
                    mainWindow?.MainWindowFrame.Navigate(new AdminPage(employee, adminRepo));
                    break;
                case 1:
                    // Manager
                    var managerRepo = new ManagerRepositoryImpl(_supabaseService);
                    mainWindow?.MainWindowFrame.Navigate(new ManagerPage(employee, managerRepo));
                    break;
                case 2:
                case 3:
                    // Worker (Waiter or Chef)
                    var workerRepo = new WorkerRepositoryImpl(_supabaseService);
                    mainWindow?.MainWindowFrame.Navigate(new WorkerPage(employee, workerRepo));
                    break;
                default:
                    MessageBox.Show("Unknown role.");
                    break;
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("Login failed: " + exception.Message);
        }
    }
}