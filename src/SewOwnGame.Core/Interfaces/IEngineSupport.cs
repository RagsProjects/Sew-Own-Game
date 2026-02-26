using SewOwnGame.Core.Enums;
using SewOwnGame.Core.Models;

namespace SewOwnGame.Core.Interfaces;

public interface IEngineSupport
{
    /// <summary>
    /// Game engine name (like: "Unity", "Unreal Engine")
    /// </summary>
    string EngineName { get; }
    
    /// <summary>
    /// Engine type
    /// </summary>
    EngineType EngineType { get; }
    
    /// <summary>
    /// Common paths where projects are saved
    /// </summary>
    string[] CommonProjectPaths { get; }
    
    /// <summary>
    /// Verify if path is a valid project for this engine
    /// </summary>
    Task<bool> IsValidProjectAsync(string path);
    
    /// <summary>
    /// Holds detailed info about the project
    /// </summary>
    Task<GameProject?> LoadProjectAsync(string path);
    
    /// <summary>
    /// Detects game engine version
    /// </summary>
    string DetectEngineVersion(string projectPath);
}