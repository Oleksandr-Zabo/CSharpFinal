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
            var employee = await loginRepository.LoginAsync(email, password);
    
            if (employee != null)
            {
                MessageBox.Show("Login successful!");
                // Navigate to the AdminPage or perform other actions
            }
            else
            {
                MessageBox.Show("Invalid email or password.");
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("Login failed: " + exception.Message);
        }
    }
}