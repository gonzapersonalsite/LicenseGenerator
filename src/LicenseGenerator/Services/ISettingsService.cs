namespace LicenseGenerator.Services;

public interface ISettingsService
{
    string AppTheme { get; set; }
    double FontSizeScaling { get; set; }
    string CurrentLanguage { get; set; }
    void Save();
    void Load();
    void ResetToDefaults();
}
