using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Platform;

namespace LicenseGenerator.Services;

public class LanguageService : ILanguageService, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly ISettingsService _settingsService;
    private Dictionary<string, string> _translations = new();
    private string _currentLanguage = "es-ES";

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set => SetLanguage(value);
    }

    public IEnumerable<string> AvailableLanguages { get; } = new[] { "es-ES", "en-US" };

    public LanguageService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _currentLanguage = _settingsService.CurrentLanguage;
        LoadLanguage(_currentLanguage);
        UpdateResources();
    }

    public string GetString(string key)
    {
        return _translations.TryGetValue(key, out var value) ? value : $"[{key}]";
    }

    public string this[string key] => GetString(key);

    public void SetLanguage(string languageCode)
    {
        if (_currentLanguage == languageCode) return;
        
        if (LoadLanguage(languageCode))
        {
            _currentLanguage = languageCode;
            UpdateResources();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }

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
