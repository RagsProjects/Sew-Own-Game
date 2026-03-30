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

    // ── Interface implementation ─────────────────────────────────
    public bool HasPermissionErrors { get; private set; }
    public string PermissionWarningMessage { get; private set; } = string.Empty;

    // ── Cache to avoid scanning the same directory  ──────────────
    private string[]? _cachedProjectPaths;
    public string[] CommonProjectPaths => _cachedProjectPaths ??= BuildProjectPaths();

    // ── Ignored folders in size calculations ─────────────────────
    private static readonly HashSet<string> _excludedDirs = new(StringComparer.OrdinalIgnoreCase) { "Library", "Temp", "obj", "bin" };

    /* ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
        Secure Scan (respect symlinks)
       ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬ */
    private static IEnumerable<string> SafeGetDirectories(string path)
    {
        var result = new List<string>();
        var stack = new Stack<string>();
        stack.Push(path);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            string[] subDirs;

            try{ subDirs = Directory.GetDirectories(current); }
            catch (UnauthorizedAccessException ex) { Console.WriteLine($"[SKIP-AUTH] {current}: {ex.Message}"); continue; }
            catch (IOException ex) { Console.WriteLine($"[SKIP-IO] {current}: {ex.Message}"); continue; }

            foreach (var dir in subDirs)
            {
                try
                {
                    if (Directory.ResolveLinkTarget(dir, false) != null)
                        continue;
                    
                    result.Add(dir);
                    stack.Push(dir);
                }
                catch (IOException ex) { Console.WriteLine($"[SKIP-LINK] {dir}: {ex.Message}"); }
            }
        }

        return result;
    }

    /* ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
        Building the paths for each OS
       ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬ */
    private string[] BuildProjectPaths()
    {
        // Resets the status of permissions for each new scan
        HasPermissionErrors = false;
        PermissionWarningMessage = string.Empty;

        var paths = new List<string>();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CollectWindowsPaths(paths);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CollectLinuxPaths(paths);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            CollectMacOSPaths(paths);
        }
        else
        {
            Console.WriteLine("[ERROR] Could't detect OS");
        }

        Console.WriteLine("[✔] Path collection complete");
        return paths.ToArray();
    }

    private void CollectWindowsPaths(List<string> paths)
    {
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var projectsPath = Path.Combine (userProfile, "Projects");

        // Usual paths to game projects
        paths.Add(Path.Combine(documents, "Unity Projects"));
        paths.Add(projectsPath);
        paths.Add(@"C:\Projects");
        paths.Add(@"D:\Projects");

        // Subfolders of documents
        TryAddSubDirectories(documents, paths, SearchOption.AllDirectories);
    }

    private void CollectLinuxPaths(List<string> paths)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var user = Environment.UserName;

        foreach (var dir in SafeGetDirectories(home))
            paths.Add(dir);

        var mediaPaths = new[]
        {
            Path.Combine("/media", user),
            Path.Combine("/run/media", user)
        };

        foreach (var mediaBase in mediaPaths)
        {
            if (!Directory.Exists(mediaBase)) continue;
            foreach (var dir in SafeGetDirectories(mediaBase))
                paths.Add(dir);
        }
    }

    private void CollectMacOSPaths(List<string> paths)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        foreach (var dir in SafeGetDirectories(home))
            paths.Add(dir);
    }

    // Adds subfolders, catching errors individually
    private void TryAddSubDirectories(string basePath, List<string> paths, SearchOption option)
    {
        if (!Directory.Exists(basePath)) return;
        try
        {
            foreach (var folder in Directory.GetDirectories(basePath, "*", option))
                paths.Add(folder);
        }
        catch (UnauthorizedAccessException)
        {
            HasPermissionErrors = true;
            PermissionWarningMessage = $"Some folders require elevated permissions and were skipped.";
            Console.WriteLine($"[⚠] Permission error scanning: {basePath}");
        }
    }

    private long CalculateDirectorySize(DirectoryInfo root)
    {
        long size = 0;
        var stack = new Stack<DirectoryInfo>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            try
            {
                size += current.GetFiles().Sum(f => f.Length);
                foreach (var dir in current.GetDirectories()
                        .Where(d => !_excludedDirs.Contains(d.Name)))
                    stack.Push(dir);
            }
            catch (UnauthorizedAccessException)
            {
                // Error in terminal and in UI
            }
        }
        return size;
    }
    
    /* ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
        Validation and loading game project
       ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬ */
    public async Task<bool> IsValidProjectAsync(string path)
    {
        /* This fix a issue when trying to detect a project folder in a Virtual Machine with Linux
        Not sure if it happens in any other occasion, but I'll fix it just to be sure */
        path = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (!Directory.Exists(path))
            return false;
            
        // It's expected that Unity have those folders
        var hasAssets = Directory.Exists(Path.Combine(path, "Assets"));
        var hasSettings = Directory.Exists(Path.Combine(path, "ProjectSettings"));
        
        return await Task.FromResult(hasAssets && hasSettings);
    }
    
    public async Task<GameProject?> LoadProjectAsync(string path)
    {
        if (!await IsValidProjectAsync(path))
            return null;
            
        var dirInfo = new DirectoryInfo(path);
        
        return new GameProject
        {
            Name = dirInfo.Name,
            Path = path,
            EngineType = EngineType,
            EngineVersion = DetectEngineVersion(path),
            LastModified = dirInfo.LastWriteTime,
            SizeInBytes = CalculateDirectorySize(dirInfo)
        };
    }
    
    public string DetectEngineVersion(string projectPath)
    {
        try
        {
            var versionFile = Path.Combine(projectPath, "ProjectSettings", "ProjectVersion.txt");
            
            if (!File.Exists(versionFile)) { return string.Empty; }

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
}