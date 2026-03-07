using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using SewOwnGame.Core.Interfaces;
using SewOwnGame.Core.Models;
using SewOwnGame.Core.Services;

namespace SewOwnGame.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectDetectionService _projectDetectionService;
    private bool _isLoading;
    private bool _hasPermissionErrors;
    private bool _showInvalidFolderError;
    private string _permissionWarningMessage = string.Empty;
    private string _floatingErrorMessage = string.Empty;
    
    public ObservableCollection<GameProject> Projects { get; }
    public bool IsEmpty => !_isLoading && Projects.Count == 0;
    public bool HasProjects => !_isLoading && Projects.Count > 0;

    public ICommand ScanProjectsCommand { get; }
    public ICommand ImportProjectCommand { get; }
    public ICommand OpenGitHubIssuesCommand { get; }
    public ICommand DismissInvalidFolderErrorCommand { get; }
    
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasProjects));
            }
        }
    }

    public bool HasPermissionErrors
    {
        get => _hasPermissionErrors;
        set
        {
            if (_hasPermissionErrors != value)
            {
                _hasPermissionErrors = value;
                OnPropertyChanged(nameof(HasPermissionErrors));
            }
        }
    }

    public string PermissionWarningMessage
    {
        get => _permissionWarningMessage;
        set
        {
            if (_permissionWarningMessage != value)
            {
                _permissionWarningMessage = value;
                OnPropertyChanged(nameof(PermissionWarningMessage));
            }
        }
    }

    public string FloatingErrorMessage
    {
        get => _floatingErrorMessage;
        set
        {
            if (_floatingErrorMessage != value)
            {
                _floatingErrorMessage = value;
                OnPropertyChanged(nameof(FloatingErrorMessage));
            }
        }
    }

    public bool ShowInvalidFolderError
    {
        get => _showInvalidFolderError;
        set
        {
            if (_showInvalidFolderError != value)
            {
                _showInvalidFolderError = value;
                OnPropertyChanged(nameof(ShowInvalidFolderError));
            }
        }
    }

    private Task OpenGitHubIssuesAsync()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/RagsProjects/Sew-Own-Game/issues/new/choose",
            UseShellExecute = true
        });
        return Task.CompletedTask;
    }

    private async Task ScanProjectsAsync()
    {
        IsLoading = true;
        Projects.Clear();
        HasPermissionErrors = false;

        try
        {
            var detectedProjects = await Task.Run(async () => await _projectDetectionService.DetectProjectsAsync());

            if (_projectDetectionService is UniversalProjectDetectionService universalService && universalService.HasPermissionErrors)
            {
                HasPermissionErrors = true;
                PermissionWarningMessage = universalService.PermissionWarningMessage;
            }

            foreach (var project in detectedProjects)
            {
                Projects.Add(project);
            }
        }
        catch (Exception ex)
        {
            FloatingErrorMessage = "A permission error occurred while scanning. Reopen SOG as admin or change folder permissions.";
            ShowInvalidFolderError = true;

            /* This was supposed to be a crash log
            but this catch itself fix the crashing
            so it will dignose the crash in terminal
            even if it doesn't crash. I'll leave it here because it may be useful someday*/
            Console.WriteLine($"[CRASH] {ex.GetType().FullName}: {ex.Message}");
            Console.WriteLine($"[STACK] {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[INNER] {ex.InnerException.GetType().FullName}: {ex.InnerException.Message}");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public MainWindowViewModel()
    {
        _projectDetectionService = new UniversalProjectDetectionService();
        Projects = new ObservableCollection<GameProject>();
        Projects.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(HasProjects));
        };
        
        ScanProjectsCommand = new AsyncCommand(ScanProjectsAsync);
        ImportProjectCommand = new AsyncCommand(ImportProjectAsync);
        OpenGitHubIssuesCommand = new AsyncCommand(OpenGitHubIssuesAsync);
        DismissInvalidFolderErrorCommand = new AsyncCommand(() => 
        {
            ShowInvalidFolderError = false;
            return Task.CompletedTask;
        });
    }

    private async Task ImportProjectAsync()
    {
        ShowInvalidFolderError = false;

        var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;
        
        if (mainWindow == null) return;
        
        var folders = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Game Project Folder",
            AllowMultiple = false
        });
        
        if (folders.Count > 0)
        {
            var folderPath = folders[0].Path.LocalPath;
            
            IsLoading = true;
            
            try
            {
                var project = await _projectDetectionService.LoadProjectAsync(folderPath);
                
                if (project != null)
                {
                    // Verify if is already on the list
                    if (!Projects.Any(p => p.Path == project.Path))
                    {
                        Projects.Add(project);
                    }
                }
                else
                {
                    Console.WriteLine("Not a valid game project folder!");
                    FloatingErrorMessage = "Not a valid game project folder!";
                    ShowInvalidFolderError = true;
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

public class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private bool _isExecuting;
    
    public event EventHandler? CanExecuteChanged;
    
    public AsyncCommand(Func<Task> execute)
    {
        _execute = execute;
    }
    
    public bool CanExecute(object? parameter) => !_isExecuting;
    
    public async void Execute(object? parameter)
    {
        if (_isExecuting) return;
        
        _isExecuting = true;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        
        try
        {
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}