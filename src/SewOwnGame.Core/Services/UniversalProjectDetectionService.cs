using SewOwnGame.Core.Interfaces;
using SewOwnGame.Core.Models;

namespace SewOwnGame.Core.Services;

public class UniversalProjectDetectionService : IProjectDetectionService
{
    private readonly EngineManager _engineManager;

    public bool HasPermissionErrors { get; private set; }
    public string PermissionWarningMessage { get; private set; } = string.Empty;

    public UniversalProjectDetectionService()
    {
        _engineManager = new EngineManager();
    }

    public async Task<IEnumerable<GameProject>> DetectProjectsAsync()
    {
        var projects = new List<GameProject>();
        HasPermissionErrors = false;

        foreach (var engine in _engineManager.GetAllEngines())
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

            if (engine is UnityEngineSupport unityEngine && unityEngine.HasPermissionErrors)
            {
                HasPermissionErrors = true;
                PermissionWarningMessage =$"Make sure your {engine.EngineName} project path doesn't have high permissions, or reopen SOG as admin.";
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