using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using CSharpFinalCore;

namespace CSharpFinal;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Navigate to the initial page (e.g., SplashWindow) when the window is loaded
        MainWindowFrame.NavigationService?.Navigate(new Pages.SplashWindow());
    }
}