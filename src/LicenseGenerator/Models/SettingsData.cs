using System.Text.Json.Serialization;

namespace LicenseGenerator.Models;

/// <summary>
/// Data contract for application settings, placed in Models namespace 
/// to avoid obfuscation/trimming issues.
/// </summary>
public class SettingsData
{
    [JsonPropertyName("AppTheme")]
    public string AppTheme { get; set; } = "System";

    [JsonPropertyName("FontSizeScaling")]
    public double FontSizeScaling { get; set; } = 1.0;

    [JsonPropertyName("CurrentLanguage")]
    public string CurrentLanguage { get; set; } = string.Empty;

    [JsonPropertyName("ShowGreeting")]
    public bool ShowGreeting { get; set; } = true;

    // Rotating greeting indices per time slot
    [JsonPropertyName("LastGreetingIndexMorning")]
    public int LastGreetingIndexMorning { get; set; } = -1;

    [JsonPropertyName("LastGreetingIndexAfternoon")]
    public int LastGreetingIndexAfternoon { get; set; } = -1;

    [JsonPropertyName("LastGreetingIndexEvening")]
    public int LastGreetingIndexEvening { get; set; } = -1;

    [JsonPropertyName("LastGreetingIndexNight")]
    public int LastGreetingIndexNight { get; set; } = -1;
}
