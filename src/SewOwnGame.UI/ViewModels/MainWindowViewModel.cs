using System;
using System.Linq;
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
    
    public ObservableCollection<GameProject> Projects { get; }
    public bool IsEmpty => !_isLoading && Projects.Count == 0;
    public bool HasProjects => !_isLoading && Projects.Count > 0;
    public ICommand ScanProjectsCommand { get; }
    public ICommand ImportProjectCommand { get; }
    
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
    }
    
    private async Task ScanProjectsAsync()
    {
        IsLoading = true;
        Projects.Clear();
        
        try
        {
            var detectedProjects = await Task.Run(async () => 
                await _projectDetectionService.DetectProjectsAsync());
            
            foreach (var project in detectedProjects)
            {
                Projects.Add(project);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

private async Task ImportProjectAsync()
    {
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