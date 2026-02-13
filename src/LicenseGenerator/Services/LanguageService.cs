using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Globalization;
using Avalonia;
using Avalonia.Platform;

namespace LicenseGenerator.Services;

public class LanguageService : ILanguageService, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly ISettingsService _settingsService;
    private Dictionary<string, string> _translations = new();
    private string _currentLanguage = "en-US";
    private readonly List<string> _availableLanguages = new();

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set => SetLanguage(value);
    }

    public IEnumerable<string> AvailableLanguages => _availableLanguages;

    public LanguageService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        
        DiscoverAvailableLanguages();
        
        // Determine initial language
        string targetLanguage = _settingsService.CurrentLanguage;
        
        // If no language is saved or it's not available, try system language
        if (string.IsNullOrEmpty(targetLanguage) || !_availableLanguages.Contains(targetLanguage))
        {
            targetLanguage = DetectSystemLanguage();
        }

        _currentLanguage = targetLanguage;
        LoadLanguage(_currentLanguage);
        UpdateResources();
    }

    private void DiscoverAvailableLanguages()
    {
        _availableLanguages.Clear();
        try
        {
            // Discover .json files in Assets/Languages/ via AssetLoader
            var assets = AssetLoader.GetAssets(new Uri("avares://LicenseGenerator/Assets/Languages/"), null);
            foreach (var asset in assets)
            {
                var fileName = Path.GetFileNameWithoutExtension(asset.LocalPath);
                if (asset.LocalPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    _availableLanguages.Add(fileName);
                }
            }
        }
        catch
        {
            // Fallback to hardcoded list if discovery fails
            _availableLanguages.AddRange(new[] { "en-US", "es-ES" });
        }

        if (!_availableLanguages.Any())
        {
            _availableLanguages.Add("en-US"); // Emergency fallback
        }
    }

    private string DetectSystemLanguage()
    {
        var culture = CultureInfo.CurrentUICulture;
        var systemLang = culture.Name; // e.g. "es-ES"
        
        // Try exact match (e.g., "es-ES")
        if (_availableLanguages.Contains(systemLang))
        {
            return systemLang;
        }

        // Try parent match (e.g., "es" if "es-ES" or "es-MX" is available)
        var parentMatch = _availableLanguages.FirstOrDefault(l => 
            l.StartsWith(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase));
        
        if (parentMatch != null)
        {
            return parentMatch;
        }

        // Default fallback
        return _availableLanguages.Contains("en-US") ? "en-US" : _availableLanguages.First();
    }

    public void SetLanguage(string languageCode)
    {
        // If languageCode is empty or null, we should use system language
        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = DetectSystemLanguage();
        }

        if (_currentLanguage == languageCode) return;
        
        if (LoadLanguage(languageCode))
        {
            _currentLanguage = languageCode;
            UpdateResources();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }

    public string GetString(string key)
    {
        return _translations.TryGetValue(key, out var value) ? value : $"[{key}]";
    }

    public string this[string key] => GetString(key);

    private bool LoadLanguage(string languageCode)
    {
        try
        {
            var uri = new Uri($"avares://LicenseGenerator/Assets/Languages/{languageCode}.json");
            using var stream = AssetLoader.Open(uri);
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void UpdateResources()
    {
        if (Application.Current == null) return;

        foreach (var translation in _translations)
        {
            Application.Current.Resources[translation.Key] = translation.Value;
        }
    }
}
