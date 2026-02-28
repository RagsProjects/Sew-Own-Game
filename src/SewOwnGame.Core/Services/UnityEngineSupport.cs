using System.Runtime.InteropServices;
using SewOwnGame.Core.Enums;
using SewOwnGame.Core.Interfaces;
using SewOwnGame.Core.Models;

namespace SewOwnGame.Core.Services;

public class UnityEngineSupport : IEngineSupport
{
    public string EngineName => "Unity";
    public EngineType EngineType => EngineType.Unity;
    
    public string[] CommonProjectPaths
    {
        get
        {
            var paths = new List<string>();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows
                paths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Unity Projects"));
                paths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Projects"));
                paths.Add(@"C:\Projects");
                paths.Add(@"D:\Projects");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                // Linux include variations on different languages
                paths.Add(Path.Combine(home, "Documents", "Unity Projects"));
                paths.Add(Path.Combine(home, "Documents"));
                paths.Add(Path.Combine(home, "Documentos"));                    // Portuguese
                paths.Add(Path.Combine(home, "Documentos", "Unity Projects"));  // Portuguese
                paths.Add(Path.Combine(home, "Projects"));
                paths.Add(Path.Combine(home, "UnityProjects"));
                paths.Add(Path.Combine(home, "projects"));
                paths.Add(Path.Combine(home, "unity-projects"));
                paths.Add(Path.Combine(home));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                paths.Add(Path.Combine(home, "Documents", "Unity Projects"));
                paths.Add(Path.Combine(home, "Projects"));
                paths.Add(Path.Combine(home, "UnityProjects"));
            }
            
            return paths.ToArray();
        }
    }
    
    public async Task<bool> IsValidProjectAsync(string path)
    {
        if (!Directory.Exists(path))
            return false;
            
        // Unity MUST have those folders
        var hasAssetsFolder = Directory.Exists(Path.Combine(path, "Assets"));
        var hasProjectSettingsFolder = Directory.Exists(Path.Combine(path, "ProjectSettings"));
        
        return await Task.FromResult(hasAssetsFolder && hasProjectSettingsFolder);
    }
    
    public async Task<GameProject?> LoadProjectAsync(string path)
    {
        if (!await IsValidProjectAsync(path))
            return null;
            
        var dirInfo = new DirectoryInfo(path);
        long size = CalculateDirectorySize(dirInfo);
        string version = DetectEngineVersion(path);
        
        return new GameProject
        {
            Name = dirInfo.Name,
            Path = path,
            EngineType = EngineType,
            EngineVersion = version,
            LastModified = dirInfo.LastWriteTime,
            SizeInBytes = size
        };
    }
    
    public string DetectEngineVersion(string projectPath)
    {
        try
        {
            var versionFile = Path.Combine(projectPath, "ProjectSettings", "ProjectVersion.txt");
            
            if (File.Exists(versionFile))
            {
                var lines = File.ReadAllLines(versionFile);
                foreach (var line in lines)
                {
                    if (line.StartsWith("m_EditorVersion:"))
                    {
                        return line.Split(':')[1].Trim();
                    }
                }
            }
        }
        catch
        {
            // Ignore errors
        }
        
        return "Unknown";
    }
    
    private long CalculateDirectorySize(DirectoryInfo directory)
    {
        long size = 0;
        
        try
        {
            foreach (var file in directory.GetFiles())
            {
                size += file.Length;
            }
            
            foreach (var dir in directory.GetDirectories())
            {
                if (dir.Name != "Library" && dir.Name != "Temp" && dir.Name != "obj" && dir.Name != "bin")
                {
                    size += CalculateDirectorySize(dir);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore folder with not enough permissions
        }
        
        return size;
    }
}