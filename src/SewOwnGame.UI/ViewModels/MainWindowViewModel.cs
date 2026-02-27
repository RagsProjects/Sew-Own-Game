using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
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
    
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }
    
    public ICommand ScanProjectsCommand { get; }
    
    public MainWindowViewModel()
    {
        _projectDetectionService = new UniversalProjectDetectionService();
        Projects = new ObservableCollection<GameProject>();
        ScanProjectsCommand = new AsyncCommand(ScanProjectsAsync);
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