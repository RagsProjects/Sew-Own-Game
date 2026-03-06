using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace SewOwnGame.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        Console.WriteLine("Avalonia app is launching...");
        AvaloniaXamlLoader.Load(this);
        Console.WriteLine("Avalonia launched!");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine("Framework booting...");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Console.WriteLine("Creating MainWindow...");
            desktop.MainWindow = new MainWindow();
            Console.WriteLine($"MainWindow Created: {desktop.MainWindow != null}");
        }
        else
        {
            Console.WriteLine("ERROR: MainWindows couldn't be created");
        }

        base.OnFrameworkInitializationCompleted();
        Console.WriteLine("Framework Initializated!");
    }
}