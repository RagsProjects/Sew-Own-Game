using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
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
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }
    
    public ICommand ScanProjectsCommand { get; }
    
    public MainWindowViewModel()
    {
        _projectDetectionService = new UnityProjectDetectionService();
        Projects = new ObservableCollection<GameProject>();
        ScanProjectsCommand = ReactiveCommand.CreateFromTask(ScanProjectsAsync);
    }
    
    private async Task ScanProjectsAsync()
    {
        IsLoading = true;
        Projects.Clear();
        
        try
        {
            var detectedProjects = await _projectDetectionService.DetectProjectsAsync();
            
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