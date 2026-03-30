using System;
using System.IO;
using System.Threading.Tasks;
using NetCoreAudio;

namespace SewOwnGame.UI.Services;

public class AudioService
{
    private readonly Player _player = new();
    private readonly string _clickSfx;
    private readonly string _popupSfx;

    public AudioService()
    {
        var basePath = AppContext.BaseDirectory;
        _clickSfx = Path.Combine(basePath, "Assets", "SFX", "clickSfx.mp3");
        _popupSfx = Path.Combine(basePath, "Assets", "SFX", "popupSfx.mp3");
    }

    public async Task PlayClickAsync()
    {
        try { await _player.Play(_clickSfx); }
        catch { /* Ignore audio errors */ }
    }

    public async Task PlayPopupAsync()
    {
        try { await _player.Play(_popupSfx); }
        catch { /* Ignore audio errors */ }
    }
}