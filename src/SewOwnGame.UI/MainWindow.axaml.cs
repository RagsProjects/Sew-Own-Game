using Avalonia.Controls;
using SewOwnGame.UI.ViewModels;

namespace SewOwnGame.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}