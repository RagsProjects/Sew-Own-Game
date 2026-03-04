using SewOwnGame.Core.Interfaces;
using SewOwnGame.Core.Models;

namespace SewOwnGame.Core.Services;

public class UniversalProjectDetectionService : IProjectDetectionService
{
    private readonly EngineManager _engineManager;
    private List<IEngineSupport> _engines;

    public bool HasPermissionErrors { get; private set; }
    
    public UniversalProjectDetectionService()
    {
        _engineManager = new EngineManager();
    }
    
    public async Task<IEnumerable<GameProject>> DetectProjectsAsync()
    {
        var projects = new List<GameProject>();
        var engines = _engineManager.GetAllEngines();

    public async Task<List<GameProject>> DetectProjectsAsync()
    {
        var projects = new List<GameProject>();
        HasPermissionErrors = false;

        foreach (var engine in _engines)
        {
            var paths = engine.CommonProjectPaths;
            
            // Verify for permission errors
            if (engine is UnityEngineSupport unityEngine && unityEngine.HasPermissionErrors)
            {
                HasPermissionErrors = true;
            }
        }
    }
        
        foreach (var engine in engines)
        {
            foreach (var basePath in engine.CommonProjectPaths)
            {
                if (!Directory.Exists(basePath))
                    continue;
                    
                var directories = Directory.GetDirectories(basePath);
                
                foreach (var dir in directories)
                {
                    if (await engine.IsValidProjectAsync(dir))
                    {
                        var project = await engine.LoadProjectAsync(dir);
                        if (project != null)
                        {
                            projects.Add(project);
                        }
                    }
                }
            }
        }
        
        return projects;
    }
    
    public async Task<bool> IsValidProjectAsync(string path)
    {
        var engine = await _engineManager.DetectEngineAsync(path);
        return engine != null;
    }
    
    public async Task<GameProject?> LoadProjectAsync(string path)
    {
        var engine = await _engineManager.DetectEngineAsync(path);
        return engine != null ? await engine.LoadProjectAsync(path) : null;
    }
}