using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading; // Add for DispatcherTimer

namespace CSharpFinal.Pages;

public partial class SplashWindow : UserControl
{
    public SplashWindow()
    {
        InitializeComponent();
        // Start a 5-second timer, then navigate to LoginPage
        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(5);
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.MainWindowFrame.NavigationService?.Navigate(new LoginPage());
        };
        timer.Start();
    }
}
