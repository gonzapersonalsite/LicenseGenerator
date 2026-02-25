using System;
using System.IO;
using System.Text.Json;
using LicenseGenerator.Models;

namespace LicenseGenerator.Services;

public class SettingsService : ISettingsService
{
    private readonly string _settingsFilePath;
    private readonly ILoggingService _loggingService;
    private string _appTheme = "System"; // Default to System
    private double _fontSizeScaling = 1.0;
    private string _currentLanguage = string.Empty; // Empty means "not set, use system"
    private bool _showGreeting = true;

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

    public bool ShowGreeting
    {
        get => _showGreeting;
        set
        {
            _showGreeting = value;
            Save();
        }
    }

    public SettingsService(ILoggingService loggingService)
    {
        _loggingService = loggingService;
        string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LicenseGenerator");
        if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
        
        _settingsFilePath = Path.Combine(baseDir, "settings.json");
        _loggingService.LogInfo($"Settings initialized. Path: {_settingsFilePath}");
        Load();
    }

    public void Save()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var data = new SettingsData
            {
                AppTheme = AppTheme,
                FontSizeScaling = FontSizeScaling,
                CurrentLanguage = CurrentLanguage,
                ShowGreeting = ShowGreeting
            };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_settingsFilePath, json);
            _loggingService.LogInfo($"Settings saved to {_settingsFilePath}");
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Error saving settings", ex);
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                _loggingService.LogDebug($"Loading settings: {json}");
                
                var data = JsonSerializer.Deserialize<SettingsData>(json);
                if (data != null)
                {
                    _appTheme = data.AppTheme ?? "Dark";
                    _fontSizeScaling = data.FontSizeScaling;
                    _currentLanguage = data.CurrentLanguage ?? string.Empty;
                    _showGreeting = data.ShowGreeting;
                    _loggingService.LogInfo($"Settings loaded from {_settingsFilePath}. Language: '{_currentLanguage}', Greeting: {_showGreeting}");
                }
            }
            else
            {
                _loggingService.LogInfo("Settings file not found, using defaults.");
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Error loading settings", ex);
        }
    }

    public void ResetToDefaults()
    {
        _appTheme = "System";
        _fontSizeScaling = 1.0;
        _currentLanguage = string.Empty;
        _showGreeting = true;
        Save();
    }
}
