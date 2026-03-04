using System.Runtime.InteropServices;
using System.IO;
using SewOwnGame.Core.Enums;
using SewOwnGame.Core.Interfaces;
using SewOwnGame.Core.Models;

namespace SewOwnGame.Core.Services;

public class UnityEngineSupport : IEngineSupport
{
    public string EngineName => "Unity";
    public EngineType EngineType => EngineType.Unity;
    public bool HasPermissionErrors { get; private set; }
    
    public string[] CommonProjectPaths
    {
        get
        {
            Console.WriteLine("Detecting OS...");

            var paths = new List<string>();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows
                Console.WriteLine("Searching files...");
                
                var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var projectsPath = Path.Combine(userProfile, "Projects");
                
                paths.Add(Path.Combine(documentsPath, "Unity Projects"));
                paths.Add(projectsPath);
                paths.Add(@"C:\Projects");
                paths.Add(@"D:\Projects");
                
                try
                {
                    if (Directory.Exists(documentsPath))
                    {
                        var docSubFolders = Directory.GetDirectories(documentsPath, "*", SearchOption.AllDirectories);
                        foreach (var folder in docSubFolders)
                        {
                            paths.Add(folder);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    HasPermissionErrors = true;
                    Console.WriteLine("[⚠] Found folders with admin privileges");
                }
                
                try
                {
                    if (Directory.Exists(projectsPath))
                    {
                        var projectSubFolders = Directory.GetDirectories(projectsPath, "*", SearchOption.AllDirectories);
                        foreach (var folder in projectSubFolders)
                        {
                            paths.Add(folder);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    HasPermissionErrors = true;
                    Console.WriteLine("[⚠] Found folders with admin privileges");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux
                Console.WriteLine("Searching files...");
                
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var user = Environment.UserName;
                var homePath = Path.Combine(home);
                var mediaPath = Path.Combine("/media", user);

                try
                {
                    if (Directory.Exists(homePath))
                    {
                        var homeSubFolders = Directory.GetDirectories(homePath, "*", SearchOption.AllDirectories);

                        // And add to list
                        foreach (var homeSubFolder in homeSubFolders)
                        {
                            paths.Add(homeSubFolder);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    HasPermissionErrors = true;
                    Console.WriteLine("[⚠] Found folders with admin privileges");
                }

                try
                {
                    if (Directory.Exists(mediaPath))
                    {
                        var mediaSubFolders = Directory.GetDirectories(mediaPath, "*", SearchOption.AllDirectories);

                        // And add to list
                        foreach (var mediaSubFolder in mediaSubFolders)
                        {
                            paths.Add(mediaSubFolder);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    HasPermissionErrors = true;
                    Console.WriteLine("[⚠] Found folders with admin privileges");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS
                Console.WriteLine("Searching files...");

                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var documentsPath = Path.Combine(home, "Documents");
                var projectsPath = Path.Combine(home, "Projects");
                
                paths.Add(Path.Combine(documentsPath, "Unity Projects"));
                paths.Add(projectsPath);
                paths.Add(Path.Combine(home, "UnityProjects"));
                
                try
                {
                    if (Directory.Exists(documentsPath))
                    {
                        var docSubFolders = Directory.GetDirectories(documentsPath, "*", SearchOption.AllDirectories);
                        foreach (var docFolder in docSubFolders)
                        {
                            paths.Add(docFolder);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    HasPermissionErrors = true;
                    Console.WriteLine("[⚠] Found folders with admin privileges");
                }
                
                try
                {
                    if (Directory.Exists(projectsPath))
                    {
                        var projectSubFolders = Directory.GetDirectories(projectsPath, "*", SearchOption.AllDirectories);
                        foreach (var projectFolder in projectSubFolders)
                        {
                            paths.Add(projectFolder);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    HasPermissionErrors = true;
                    Console.WriteLine("[⚠] Found folders with admin privileges");
                }
            }

            else
            {
                // Implement here: error message to UI
                Console.WriteLine("Error: Couldn't detect user's OS");
            }
            
            return paths.ToArray();
            Console.WriteLine("[✔] Search Succeeded");
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