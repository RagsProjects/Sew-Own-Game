using SewOwnGame.Core.Services;

namespace SewOwnGame.Tests;

public class UnityEngineSupportTests
{
    [Fact]
    public async Task IsValidProjectAsync_ValidProject_ReturnsTrue()
    {
        // Arrange
        var support = new UnityEngineSupport();
        var tempDir = Directory.CreateTempSubdirectory();
        Directory.CreateDirectory(Path.Combine(tempDir.FullName, "Assets"));
        Directory.CreateDirectory(Path.Combine(tempDir.FullName, "ProjectSettings"));

        // Act
        var result = await support.IsValidProjectAsync(tempDir.FullName);

        // Assert
        Assert.True(result);
        tempDir.Delete(recursive: true);
    }

    [Fact]
    public async Task IsValidProjectAsync_MissingAssets_ReturnsFalse()
    {
        var support = new UnityEngineSupport();
        var tempDir = Directory.CreateTempSubdirectory();
        // Só ProjectSettings, sem Assets
        Directory.CreateDirectory(Path.Combine(tempDir.FullName, "ProjectSettings"));

        var result = await support.IsValidProjectAsync(tempDir.FullName);

        Assert.False(result);
        tempDir.Delete(recursive: true);
    }

    [Fact]
    public void DetectEngineVersion_ValidFile_ReturnsVersion()
    {
        var support = new UnityEngineSupport();
        var tempDir = Directory.CreateTempSubdirectory();
        var settingsDir = Directory.CreateDirectory(Path.Combine(tempDir.FullName, "ProjectSettings"));
        File.WriteAllText(
            Path.Combine(settingsDir.FullName, "ProjectVersion.txt"),
            "m_EditorVersion: 2022.3.10f1\nm_EditorVersionWithRevision: 2022.3.10f1 (...)");

        var version = support.DetectEngineVersion(tempDir.FullName);

        Assert.Equal("2022.3.10f1", version);
        tempDir.Delete(recursive: true);
    }
}