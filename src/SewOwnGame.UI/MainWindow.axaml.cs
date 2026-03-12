using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Platform;
using System;
using System.Runtime.InteropServices;
using SewOwnGame.UI.ViewModels;

namespace SewOwnGame.UI;

public partial class MainWindow : Window
{
    private bool _isSettingsOpen = false;
    private bool _isDarkTheme = true;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        AddHandler(Button.ClickEvent, OnAnyButtonClick, RoutingStrategies.Bubble, handledEventsToo: true);

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Topmost = true;
        Topmost = false;

        /* Desktop Icon 
        Use .ico for Windows and .png for other platforms*/
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Icon = new WindowIcon(AssetLoader.Open(
                new Uri("avares://SewOwnGame.UI/Assets/Build/Win/sogIcon.ico")));
        }
        else
        {
            Icon = new WindowIcon(AssetLoader.Open(
                new Uri("avares://SewOwnGame.UI/Assets/UI/sogIcon.png")));
        }
    }

    // ── Settings overlay ──────────────────────────────────────────────

    private void OnSettingsClick(object? sender, RoutedEventArgs e)
    {
        _isSettingsOpen = !_isSettingsOpen;
        SettingsOverlay.IsVisible = _isSettingsOpen;

        // Highlight the gear icon when panel is open
        SettingsIcon.Foreground = _isSettingsOpen
            ? Avalonia.Media.Brushes.White
            : new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#8888AA"));
    }

    private void OnOverlayBackdropClick(object? sender, RoutedEventArgs e)
    {
        _isSettingsOpen = false;
        SettingsOverlay.IsVisible = false;
    }

    // ── Theme toggle ──────────────────────────────────────────────────

    private void OnThemeToggleClick(object? sender, RoutedEventArgs e)
    {
        _isDarkTheme = !_isDarkTheme;

        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant =
                _isDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
        }

        // Update icon and label inside the button
        if (_isDarkTheme)
        {
            ThemeIcon.Text = "☀️";
            ThemeLabel.Text = "Light Theme";
            ThemeSubLabel.Text = "Click me for Light Theme!";
        }
        else
        {
            ThemeIcon.Text = "🌙";
            ThemeLabel.Text = "Dark Theme";
            ThemeSubLabel.Text = "Click me for Dark Theme!";
        }
    }

    // ── Sound Effects ──────────────────────────────────────────────────

    private void OnAnyButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            _ = vm.PlayClickAsync();
        }
    }
}