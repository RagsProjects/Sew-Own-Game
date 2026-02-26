using Avalonia.Controls;
using SewOwnGame.UI.ViewModels;

namespace SewOwnGame.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        // Force window to appear on top
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Topmost = true;
        Topmost = false; // Remove topmost after appears
    }
}