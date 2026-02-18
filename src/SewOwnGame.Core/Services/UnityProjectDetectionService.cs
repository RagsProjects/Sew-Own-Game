using SewOwnGame.Core.Enums;
using SewOwnGame.Core.Interfaces;
using SewOwnGame.Core.Models;

namespace SewOwnGame.Core.Services;

public class UnityProjectDetectionService : IProjectDetectionService
{
    // Diretórios comuns onde projetos Unity são armazenados
    private readonly string[] _commonProjectPaths = 
    {
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Unity Projects"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Projects"),
        @"C:\Projects",
        @"D:\Projects"
    };
    
    public async Task<IEnumerable<GameProject>> DetectProjectsAsync()
    {
        var projects = new List<GameProject>();
        
        foreach (var basePath in _commonProjectPaths)
        {
            if (!Directory.Exists(basePath))
                continue;
                
            var directories = Directory.GetDirectories(basePath);
            
            foreach (var dir in directories)
            {
                if (await IsValidProjectAsync(dir))
                {
                    var project = await LoadProjectAsync(dir);
                    if (project != null)
                    {
                        projects.Add(project);
                    }
                }
            }
        }
        
        return projects;
    }
    
    public async Task<bool> IsValidProjectAsync(string path)
    {
        if (!Directory.Exists(path))
            return false;
            
        // Projeto Unity DEVE ter essas duas pastas
        var hasAssetsFolder = Directory.Exists(Path.Combine(path, "Assets"));
        var hasProjectSettingsFolder = Directory.Exists(Path.Combine(path, "ProjectSettings"));
        
        return await Task.FromResult(hasAssetsFolder && hasProjectSettingsFolder);
    }
    
    public async Task<GameProject?> LoadProjectAsync(string path)
    {
        if (!await IsValidProjectAsync(path))
            return null;
            
        var dirInfo = new DirectoryInfo(path);
        
        // Calcula tamanho do projeto (pode ser lento para projetos grandes)
        long size = await Task.Run(() => CalculateDirectorySize(dirInfo));
        
        // Detecta versão do Unity
        string version = DetectUnityVersion(path);
        
        return new GameProject
        {
            Name = dirInfo.Name,
            Path = path,
            EngineType = EngineType.Unity,
            EngineVersion = version,
            LastModified = dirInfo.LastWriteTime,
            SizeInBytes = size
        };
    }
    
    private long CalculateDirectorySize(DirectoryInfo directory)
    {
        long size = 0;
        
        try
        {
            // Soma arquivos na pasta atual
            foreach (var file in directory.GetFiles())
            {
                size += file.Length;
            }
            
            // Recursivamente soma subpastas (exceto Library e Temp que são gerados)
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
            // Ignora pastas sem permissão
        }
        
        return size;
    }
    
    private string DetectUnityVersion(string projectPath)
    {
        try
        {
            // Unity stores the version in ./UnityProject/ProjectSettings/ProjectVersion.txt
            var versionFile = Path.Combine(projectPath, "ProjectSettings", "ProjectVersion.txt");
            
            if (File.Exists(versionFile))
            {
                var lines = File.ReadAllLines(versionFile);
                foreach (var line in lines)
                {
                    // Formato: m_EditorVersion: 2022.3.10f1
                    if (line.StartsWith("m_EditorVersion:"))
                    {
                        return line.Split(':')[1].Trim();
                    }
                }
            }
        }
        catch
        {
            // Ignore errors before read
        }
        
        return "Unknown";
    }
}