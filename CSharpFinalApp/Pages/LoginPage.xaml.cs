using System.Windows;
using System.Windows.Controls;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.Models;
using CSharpFinalData.Data.RepositoryImpl.LoginRepositoryImpl;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinal.Pages;

public partial class LoginPage : UserControl
{
    private readonly SupabaseService _supabaseService;

    public LoginPage()
    {
        InitializeComponent();
        _supabaseService = new SupabaseService();
    }

    private async void OnLoginButtonClick(object sender, RoutedEventArgs e)
    {
        var email = LoginTextBox.Text;
        var password = PasswordBox.Password;
    
        try
        {
            // Use a concrete implementation of LoginRepository
            var loginRepository = new LoginRepositoryImpl(_supabaseService);
            var user = await loginRepository.LoginAsync(email, password);
            
            var employee = await loginRepository.GetEmployeeByUserAsync(email, password);

            MessageBox.Show("Login successful!");
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            // Navigate to the Page based on the role switch-case - Admin- 0 (AdminPage) Manager-1 (ManagerPage) Waiter-2 (WorkerPage) Chef- 3(WorkerPage) else-unknown
            switch (employee.RoleId)
            {
                case 0:
                    // Navigate to AdminPage
                    
                    mainWindow?.MainWindowFrame.Navigate(new AdminPage(employee, _supabaseService));
                    break;
                case 1:
                    // Navigate to ManagerPage
                    mainWindow?.MainWindowFrame.Navigate(new ManagerPage(employee, _supabaseService));
                    break;
                case 2:
                    // Navigate to WorkerPage
                    mainWindow?.MainWindowFrame.Navigate(new WorkerPage(employee, _supabaseService));
                    break;
                case 3:
                    // Navigate to WorkerPage
                    mainWindow?.MainWindowFrame.Navigate(new WorkerPage(employee, _supabaseService));
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