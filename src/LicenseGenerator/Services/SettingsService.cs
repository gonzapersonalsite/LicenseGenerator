using System;
using System.IO;
using System.Text.Json;

namespace LicenseGenerator.Services;

public class SettingsService : ISettingsService
{
    private readonly string _settingsFilePath;
    private string _appTheme = "System"; // Default to System
    private double _fontSizeScaling = 1.0;
    private string _currentLanguage = string.Empty; // Empty means "not set, use system"

    public string AppTheme
    {
        get => _appTheme;
        set 
        { 
            _appTheme = value;
            Save();
        }
    }

    public double FontSizeScaling
    {
        get => _fontSizeScaling;
        set
        {
            _fontSizeScaling = value;
            Save();
        }
    }

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            _currentLanguage = value;
            Save();
        }
    }

    public SettingsService()
    {
        string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LicenseGenerator");
        if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
        
        _settingsFilePath = Path.Combine(baseDir, "settings.json");
        Load();
    }

    public void Save()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(new { AppTheme, FontSizeScaling, CurrentLanguage }, options);
            File.WriteAllText(_settingsFilePath, json);
        }
        catch { /* Handle error silently or log */ }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                if (data.TryGetProperty("AppTheme", out var themeProperty))
                {
                    _appTheme = themeProperty.GetString() ?? "Dark";
                }
                if (data.TryGetProperty("FontSizeScaling", out var fontProperty))
                {
                    _fontSizeScaling = fontProperty.GetDouble();
                }
                if (data.TryGetProperty("CurrentLanguage", out var langProperty))
                {
                    _currentLanguage = langProperty.GetString() ?? string.Empty;
                }
            }
        }
        catch { /* Fallback to defaults */ }
    }

    public void ResetToDefaults()
    {
        _appTheme = "System";
        _fontSizeScaling = 1.0;
        _currentLanguage = string.Empty;
        Save();
    }
}
