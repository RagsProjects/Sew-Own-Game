using SewOwnGame.Core.Models;

namespace SewOwnGame.Core.Interfaces;

public interface IProjectDetectionService
{
    /// <summary>
    /// Detect all game engine projects in system
    /// </summary>
    Task<IEnumerable<GameProject>> DetectProjectsAsync();
    
    /// <summary>
    /// Verify if directory is a valid project
    /// </summary>
    Task<bool> IsValidProjectAsync(string path);
    
    /// <summary>
    /// Holds detailed information of a project
    /// </summary>
    Task<GameProject?> LoadProjectAsync(string path);
}