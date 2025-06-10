using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading; // Add for DispatcherTimer

namespace CSharpFinal.Pages;

public partial class SplashWindow : UserControl
{
    public SplashWindow()
    {
        InitializeComponent();
        // Ensure SupabaseService keys are loaded before navigating to the LoginPage
        Loaded += async (s, e) =>
        {
            try
            {
                await CSharpFinalData.Data.Source.Remote.SupabaseDB.SupabaseService.InitKeysAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize SupabaseService keys: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            // Start a 5-second timer, then navigate to the LoginPage
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += (s2, e2) =>
            {
                timer.Stop();
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.MainWindowFrame.NavigationService?.Navigate(new LoginPage());
            };
            timer.Start();
        };
    }
}
