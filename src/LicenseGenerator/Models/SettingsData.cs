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
}
