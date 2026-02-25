namespace LicenseGenerator.Services;

public interface ISettingsService
{
    string AppTheme { get; set; }
    double FontSizeScaling { get; set; }
    string CurrentLanguage { get; set; }
    bool ShowGreeting { get; set; }
    int LastGreetingIndexMorning { get; set; }
    int LastGreetingIndexAfternoon { get; set; }
    int LastGreetingIndexEvening { get; set; }
    int LastGreetingIndexNight { get; set; }
    void Save();
    void Load();
    void ResetToDefaults();
}
