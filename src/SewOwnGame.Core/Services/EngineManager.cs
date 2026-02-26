using SewOwnGame.Core.Interfaces;

namespace SewOwnGame.Core.Services;

public class EngineManager
{
    private readonly List<IEngineSupport> _engines;
    
    public EngineManager()
    {
        _engines = new List<IEngineSupport>
        {
            new UnityEngineSupport(),
            // Soon: new UnrealEngineSupport(),
            // Soon: new GodotEngineSupport(),
        };
    }
    
    /// <summary>
    /// Return all supported game engines
    /// </summary>
    public IEnumerable<IEngineSupport> GetAllEngines()
    {
        return _engines;
    }
    
    /// <summary>
    /// Detects which engine can use this path
    /// </summary>
    public async Task<IEngineSupport?> DetectEngineAsync(string path)
    {
        foreach (var engine in _engines)
        {
            if (await engine.IsValidProjectAsync(path))
            {
                return engine;
            }
        }
        
        return null;
    }
}