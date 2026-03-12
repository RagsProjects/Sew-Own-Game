using System;
using System.Runtime.InteropServices;
using System.IO;
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
using SewOwnGame.UI.Services;

namespace SewOwnGame.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectDetectionService _projectDetectionService;
    private readonly SettingsService _settingsService;

    private bool _isLoading;
    private bool _hasPermissionErrors;
    private bool _showInvalidFolderError;
    private bool _showRenameOverlay;
    private readonly AudioService _audioService = new();
    private string _renameInputText = string.Empty;
    private string _renameErrorMessage = string.Empty;
    private string _permissionWarningMessage = string.Empty;
    private string _floatingErrorMessage = string.Empty;
    private AppSettings _appSettings;
    private GameProject? _projectBeingRenamed;

    //      Game Engines Paths      \\
    private string _unityEditorPath = string.Empty;
    private string _unrealEditorPath = string.Empty;
    private string _godotEditorPath = string.Empty;
    private string _gmEditorPath = string.Empty;
    private string _strideEditorPath = string.Empty;
    private string _o3deEditorPath = string.Empty;

    public ObservableCollection<GameProject> Projects { get; }
    public Task PlayClickAsync() => _audioService.PlayClickAsync();
    public bool IsEmpty => !_isLoading && Projects.Count == 0;
    public bool HasProjects => !_isLoading && Projects.Count > 0;

    public ICommand ScanProjectsCommand { get; }
    public ICommand ImportProjectCommand { get; }
    public ICommand OpenGitHubIssuesCommand { get; }
    public ICommand DismissInvalidFolderErrorCommand { get; }
    public ICommand OpenProjectCommand { get; }
    public ICommand OpenInExplorerCommand { get; }
    public ICommand RenameProjectCommand { get; }
    public ICommand DeleteProjectCommand { get; }
    public ICommand ConfirmRenameCommand { get; }
    public ICommand CancelRenameCommand { get; }
    
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
                if (value) _ = _audioService.PlayPopupAsync();
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

    public bool ShowRenameOverlay
    {
        get => _showRenameOverlay;
        set
        {
            if (_showRenameOverlay != value)
            {
                _showRenameOverlay = value;
                OnPropertyChanged(nameof(ShowRenameOverlay));
                if (value) _ = _audioService.PlayPopupAsync();
            }
        }
    }

    public string RenameInputText
    {
        get => _renameInputText;
        set
        {
            if (_renameInputText != value)
            {
                _renameInputText = value;
                OnPropertyChanged(nameof(RenameInputText));
                RenameErrorMessage = string.Empty;
            }
        }
    }

    public string RenameErrorMessage
    {
        get => _renameErrorMessage;
        set
        {
            if(_renameErrorMessage != value)
            {
                _renameErrorMessage = value;
                OnPropertyChanged(nameof(RenameErrorMessage));
                OnPropertyChanged(nameof(HasRenameError));
            }
        }
    }

    public bool HasRenameError => !string.IsNullOrEmpty(_renameErrorMessage);

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
            FloatingErrorMessage = "A permission error occurred while scanning. Make sure your project folders have the correct permissions";
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

    private async Task BrowseUnityEditorPathAsync()
    {
        var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;
        if (mainWindow == null) return;

        var folders = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Unity Editor Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            UnityEditorPath = folders[0].Path.LocalPath;
        }
    }

    private Task OpenProjectAsync(GameProject? project)
    {
        if (project == null) return Task.CompletedTask;

        string executablePath;

        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            executablePath = Path.Combine(_unityEditorPath, project.EngineVersion, "Editor", "Unity.exe");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            executablePath = Path.Combine(_unityEditorPath, project.EngineVersion, "Unity.app", "Contents", "MacOS", "Unity");
        }
        else
        {
            executablePath = Path.Combine(_unityEditorPath, project.EngineVersion, "Editor", "Unity");
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = $"-projectPath \"{project.Path}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            FloatingErrorMessage = $"Could not open Unity editor. Check the Editor Path in Settings -> Unity Settings.\n{ex.Message}";
            ShowInvalidFolderError = true;
        }

        return Task.CompletedTask;
    }

    private Task OpenInExplorerAsync(GameProject? project)
    {
        if (project == null) return Task.CompletedTask;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start("explorer.exe", project.Path);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", project.Path);
        }
        else
        {
            Process.Start("xdg-open", project.Path);
        }

        return Task.CompletedTask;
    }

    private Task RenameProjectAsync(GameProject? project)
    {
        if (project == null) return Task.CompletedTask;

        _projectBeingRenamed = project;
        RenameInputText = project.Name;
        RenameErrorMessage = string.Empty;
        ShowRenameOverlay = true;

        return Task.CompletedTask;
    }

    private Task ConfirmRenameAsync()
    {
        if (_projectBeingRenamed == null) return Task.CompletedTask;

        // Validação: vazio
        if (string.IsNullOrWhiteSpace(RenameInputText))
        {
            RenameErrorMessage = "Sorry, we can't rename to '" + RenameInputText + "' (blank name).";
            return Task.CompletedTask;
        }

        // Validação: caracteres inválidos para nome de pasta
        var invalidChars = System.IO.Path.GetInvalidFileNameChars();
        if (RenameInputText.IndexOfAny(invalidChars) >= 0)
        {
            RenameErrorMessage = "Special characters are not allowed!";
            return Task.CompletedTask;
        }

        try
        {
            var parentDir = System.IO.Path.GetDirectoryName(_projectBeingRenamed.Path);
            if (parentDir == null) return Task.CompletedTask;

            var newPath = System.IO.Path.Combine(parentDir, RenameInputText);

            Directory.Move(_projectBeingRenamed.Path, newPath);

            _projectBeingRenamed.Name = RenameInputText;
            _projectBeingRenamed.Path = newPath;

            // Força atualização da lista
            var index = Projects.IndexOf(_projectBeingRenamed);
            if (index >= 0)
            {
                Projects.RemoveAt(index);
                Projects.Insert(index, _projectBeingRenamed);
            }

            ShowRenameOverlay = false;
        }
        catch (Exception ex)
        {
            RenameErrorMessage = $"Could not rename folder: {ex.Message}";
        }

        return Task.CompletedTask;
    }

    private Task CancelRenameAsync()
    {
        ShowRenameOverlay = false;
        RenameInputText = string.Empty;
        RenameErrorMessage = string.Empty;
        _projectBeingRenamed = null;
        return Task.CompletedTask;
    }

    private Task DeleteProjectAsync(GameProject? project)
    {
        if (project == null) return Task.CompletedTask;
        Projects.Remove(project);
        return Task.CompletedTask;
    }
    
    public MainWindowViewModel()
    {
        _settingsService = new SettingsService();
        _appSettings = _settingsService.Load();
        _unityEditorPath = _appSettings.UnityEditorPath;
        _projectDetectionService = new UniversalProjectDetectionService();

        OpenProjectCommand = new AsyncRelayCommand<GameProject>(OpenProjectAsync);
        OpenInExplorerCommand   = new AsyncRelayCommand<GameProject>(OpenInExplorerAsync);
        RenameProjectCommand    = new AsyncRelayCommand<GameProject>(RenameProjectAsync);
        ConfirmRenameCommand = new AsyncCommand(ConfirmRenameAsync);
        CancelRenameCommand = new AsyncCommand(CancelRenameAsync);
        DeleteProjectCommand    = new AsyncRelayCommand<GameProject>(DeleteProjectAsync);
        BrowseUnityEditorPathCommand = new AsyncCommand(BrowseUnityEditorPathAsync);
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

    // Game Engines Proprieties
    public string UnityEditorPath
    {
        get => _unityEditorPath;
        set
        {
            if (_unityEditorPath != value)
            {
                _unityEditorPath = value;
                OnPropertyChanged(nameof(UnityEditorPath));
                _appSettings.UnityEditorPath = value;
                _settingsService.Save(_appSettings);
            }
        }
    }

    public ICommand BrowseUnityEditorPathCommand { get; }

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

public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T?, Task> _execute;
    private bool _isExecuting;

    public event EventHandler? CanExecuteChanged;

    public AsyncRelayCommand(Func<T?, Task> execute)
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
            await _execute(parameter is T t ? t : default);
        }
        finally
        {
            _isExecuting = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}