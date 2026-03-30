using System.Runtime.InteropServices;
using System.Text.Json;
using SewOwnGame.Core.Models;

namespace SewOwnGame.Core.Services;

public class SettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SewOwnGame",
        "settings.json");

    public AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? CreateDefault();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARNING] Failed to load settings: {ex.Message}");
        }

        return CreateDefault();
    }

    public void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARNING] Failed to save settings: {ex.Message}");
        }
    }

    private static AppSettings CreateDefault()
    {
        return new AppSettings
        {
            UnityEditorPath = GetDefaultUnityEditorPath()
        };
    }

    private static string GetDefaultUnityEditorPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return @"C:\Program Files\Unity\Hub\Editor";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "/Applications/Unity/Hub/Editor";

        // Linux
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Unity", "Hub", "Editor");
    }
}