using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LicenseGenerator.Services;
using Avalonia;
using Avalonia.Styling;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace LicenseGenerator.ViewModels;

public record SettingsOption<T>(T Value, string DisplayKey);

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILanguageService _languageService;
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;

    public ILanguageService LanguageService => _languageService;

    [ObservableProperty]
    private SettingsOption<string> _selectedTheme = default!;

    [ObservableProperty]
    private SettingsOption<double> _selectedFontSize = default!;

    [ObservableProperty]
    private SettingsOption<string> _selectedLanguage = default!;

    [ObservableProperty]
    private bool _isResetConfirmOpen;

    [ObservableProperty]
    private bool _isImportConfirmOpen;

    public ObservableCollection<SettingsOption<string>> Themes { get; } = new();
    public ObservableCollection<SettingsOption<double>> FontSizes { get; } = new();
    public ObservableCollection<SettingsOption<string>> Languages { get; } = new();

    public SettingsViewModel(ISettingsService settingsService, ILanguageService languageService, IDataService dataService, IDialogService dialogService, INotificationService notificationService)
    {
        _settingsService = settingsService;
        _languageService = languageService;
        _dataService = dataService;
        _dialogService = dialogService;
        _notificationService = notificationService;

        InitializeOptions();
        LoadCurrentSettings();
    }

    private void InitializeOptions()
    {
        Themes.Clear();
        Themes.Add(new SettingsOption<string>("Light", "ThemeLight"));
        Themes.Add(new SettingsOption<string>("Dark", "ThemeDark"));
        Themes.Add(new SettingsOption<string>("System", "ThemeSystem"));

        FontSizes.Clear();
        FontSizes.Add(new SettingsOption<double>(0.8, "LangFontSizeSmall"));
        FontSizes.Add(new SettingsOption<double>(1.0, "LangFontSizeNormal"));
        FontSizes.Add(new SettingsOption<double>(1.2, "LangFontSizeLarge"));
        FontSizes.Add(new SettingsOption<double>(1.5, "LangFontSizeExtraLarge"));

        Languages.Clear();
        foreach (var langCode in _languageService.AvailableLanguages)
        {
            string labelKey = langCode switch
            {
                "es-ES" => "LangSpanish",
                "en-US" => "LangEnglish",
                "de-DE" => "LangGerman",
                _ => langCode // Fallback to the code itself for new languages
            };
            Languages.Add(new SettingsOption<string>(langCode, labelKey));
        }
    }

    private void LoadCurrentSettings()
    {
        SelectedTheme = Themes.FirstOrDefault(t => t.Value == _settingsService.AppTheme) ?? Themes[1]; // Default Dark
        SelectedFontSize = FontSizes.FirstOrDefault(f => f.Value == _settingsService.FontSizeScaling) ?? FontSizes[1]; // Default 1.0
        
        // Match the language from service (which already handled system/saved logic)
        SelectedLanguage = Languages.FirstOrDefault(l => l.Value == _languageService.CurrentLanguage) ?? Languages.FirstOrDefault();
    }

    partial void OnSelectedThemeChanged(SettingsOption<string> value)
    {
        if (value == null) return;
        if (_settingsService.AppTheme != value.Value)
        {
            _settingsService.AppTheme = value.Value;
            ApplyTheme(value.Value);
        }
    }

    partial void OnSelectedFontSizeChanged(SettingsOption<double> value)
    {
        if (value == null) return;
        if (_settingsService.FontSizeScaling != value.Value)
        {
            _settingsService.FontSizeScaling = value.Value;
            if (Application.Current is App app)
            {
                app.ApplyFontSize(value.Value);
            }
        }
    }

    partial void OnSelectedLanguageChanged(SettingsOption<string> value)
    {
        if (value == null) return;
        if (_settingsService.CurrentLanguage != value.Value)
        {
            _settingsService.CurrentLanguage = value.Value;
            _languageService.SetLanguage(value.Value);
        }
    }

    private void ApplyTheme(string theme)
    {
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = theme switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                "System" => ThemeVariant.Default,
                _ => ThemeVariant.Dark
            };
        }
    }

    [RelayCommand]
    private void ShowResetConfirm()
    {
        IsResetConfirmOpen = true;
    }

    [RelayCommand]
    private void CancelReset()
    {
        IsResetConfirmOpen = false;
    }

    [RelayCommand]
    private void ConfirmReset()
    {
        // 1. Reset service
        _settingsService.ResetToDefaults();
        
        // 2. Refresh everything
        _languageService.SetLanguage(_settingsService.CurrentLanguage);
        ApplyTheme(_settingsService.AppTheme);
        if (Application.Current is App app)
        {
            app.ApplyFontSize(_settingsService.FontSizeScaling);
        }

        // 3. Update VM state
        LoadCurrentSettings();
        IsResetConfirmOpen = false;
        _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Settings.ResetSuccess"]);
    }

    [RelayCommand]
    private async Task ExportData()
    {
        var defaultName = $"LicenseGenerator_Backup_{DateTime.Now:yyyyMMdd}.zip";
        var path = await _dialogService.SaveFileAsync(
            _languageService["Backup.ExportTitle"],
            defaultName,
            new[] { "zip" });

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                await _dataService.ExportDataAsync(path);
                _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Backup.ExportSuccess"]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Export error: {ex}");
                _notificationService.ShowError(_languageService["CommonError"], _languageService["Backup.ExportError"]);
            }
        }
    }

    [RelayCommand]
    private void ShowImportConfirm()
    {
        IsImportConfirmOpen = true;
    }

    [RelayCommand]
    private void CancelImport()
    {
        IsImportConfirmOpen = false;
    }

    [RelayCommand]
    private async Task ConfirmImport()
    {
        IsImportConfirmOpen = false;
        
        var path = await _dialogService.OpenFileAsync(
            _languageService["Backup.ImportTitle"],
            new[] { "zip" });

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                await _dataService.ImportDataAsync(path);
                _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Backup.ImportSuccess"]);
                
                // Restart app to apply changes safely
                await Task.Delay(1000); // Wait a bit for notification visibility
                System.Diagnostics.Process.Start(Environment.ProcessPath!);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Import error: {ex}");
                _notificationService.ShowError(_languageService["CommonError"], _languageService["Backup.ImportError"]);
            }
        }
    }

    [RelayCommand]
    private void OpenSupportLink()
    {
        try
        {
            var url = "https://buymeacoffee.com/gonzalomartinezgarcia";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening support link: {ex}");
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Support.OpenError"]);
        }
    }

    [RelayCommand]
    private void OpenRepositoryLink()
    {
        try
        {
            var url = "https://github.com/gonzapersonalsite/LicenseGenerator";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening repository link: {ex}");
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Repository.OpenError"]);
        }
    }
}
