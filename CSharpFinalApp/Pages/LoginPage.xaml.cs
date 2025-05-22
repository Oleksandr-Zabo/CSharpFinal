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
            // Navigate to the Page based on the role switch-case - Admin- 0 (AdminPage) Manager-1 (ManagerPage) Waiter-2 (WorkerPage) Chef- 3(WorkerPage) else- unknown
            switch (employee.RoleId)
            {
                case 0:
                    // Navigate to AdminPage
                    var adminPage = new AdminPage(employee, _supabaseService);
                    if (Application.Current.MainWindow != null)
                        Application.Current.MainWindow.Resources["MainFrame"] = adminPage;
                    break;
                case 1:
                    // Navigate to ManagerPage
                    break;
                case 2:
                case 3:
                    // Navigate to WorkerPage
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