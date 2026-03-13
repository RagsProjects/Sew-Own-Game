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

    private static IEnumerable<string> SafeGetDirectories(string path)
    {
        var result = new List<string>();
        var stack = new Stack<string>();
        stack.Push(path);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            string[] subDirs;

            try
            {
                subDirs = Directory.GetDirectories(current);
            }
            catch (UnauthorizedAccessException ex) { Console.WriteLine($"[SKIP-AUTH] {current}: {ex.Message}"); continue; }
            catch (IOException ex) { Console.WriteLine($"[SKIP-LINK] {current}: {ex.Message}"); continue; }

            foreach (var dir in subDirs)
            {
                try
                {
                    if (Directory.ResolveLinkTarget(dir, false) != null)
                        continue;
                    
                    result.Add(dir);
                    stack.Push(dir);
                }
                catch (IOException ex) { Console.WriteLine($"[SKIP-LINK] {dir}: {ex.Message}"); continue; }
            }
        }

        return result;
    }
    
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
                
                // Scan only documents folder and default project path to avoid scanning the entire user folder
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
                var runMediaPath = Path.Combine("/run/media", user);

                try
                {
                    if (Directory.Exists(homePath))
                    {
                        foreach (var homeFolder in SafeGetDirectories(homePath))
                        {
                            paths.Add(homeFolder);
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
                        foreach (var mediaFolder in SafeGetDirectories(mediaPath))
                        {
                            paths.Add(mediaFolder);
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
                    if (Directory.Exists(runMediaPath))
                    {
                        foreach (var runMediaFolder in SafeGetDirectories(runMediaPath))
                        {
                            paths.Add(runMediaFolder);
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
                var homePath = Path.Combine(home);
                
                try
                {
                    if (Directory.Exists(homePath))
                    {
                        foreach (var homeFolder in SafeGetDirectories(homePath))
                        {
                            paths.Add(homeFolder);
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
            
            Console.WriteLine("[✔] Search Succeeded");
            return paths.ToArray();
        }
    }
    
    public async Task<bool> IsValidProjectAsync(string path)
    {
        /* This fix a issue when trying to detect a project folder in a Virtual Machine with Linux
        Not sure if it happens in any other occasion, but I'll fix it just to be sure */
        path = path.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

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
            
            if (!File.Exists(versionFile))
            {
                return string.Empty;
            }

            foreach (var line in File.ReadAllLines(versionFile))
            {
                if (line.StartsWith("m_EditorVersion:"))
                    return line.Split(':', 2)[1].Trim();
            }
        }
        catch
        {
            // Ignore errors
        }
        
        return string.Empty;
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