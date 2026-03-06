using SewOwnGame.Core.Enums;

namespace SewOwnGame.Core.Models;

public class GameProject
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public EngineType EngineType { get; set; }
    public string EngineVersion { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public long SizeInBytes { get; set; }
    
    public string DisplaySize => FormatBytes(SizeInBytes);
    
    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}