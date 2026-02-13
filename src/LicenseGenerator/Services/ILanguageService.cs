using System.Collections.Generic;
using System.ComponentModel;

namespace LicenseGenerator.Services;

public interface ILanguageService : INotifyPropertyChanged
{
    string this[string key] { get; }
    string CurrentLanguage { get; set; }
    IEnumerable<string> AvailableLanguages { get; }
    string GetString(string key);
    void SetLanguage(string languageCode);
}
