using System;

namespace LicenseGenerator.Services;

public class GreetingService : IGreetingService
{
    private readonly INotificationService _notificationService;
    private readonly ILanguageService _languageService;
    private readonly ISettingsService _settingsService;
    private readonly ILoggingService _loggingService;

    public GreetingService(
        INotificationService notificationService,
        ILanguageService languageService,
        ISettingsService settingsService,
        ILoggingService loggingService)
    {
        _notificationService = notificationService;
        _languageService = languageService;
        _settingsService = settingsService;
        _loggingService = loggingService;
    }

    public void ShowSmartGreeting()
    {
        try
        {
            if (!_settingsService.ShowGreeting)
            {
                return;
            }

            var hour = DateTime.Now.Hour;
            string greetingKey;

            if (hour >= 5 && hour < 12)
            {
                greetingKey = "Greeting.Morning";
            }
            else if (hour >= 12 && hour < 19)
            {
                greetingKey = "Greeting.Afternoon";
            }
            else if (hour >= 19 && hour < 24)
            {
                greetingKey = "Greeting.Evening";
            }
            else
            {
                greetingKey = "Greeting.Night";
            }

            var title = _languageService[greetingKey];
            var slot = greetingKey.Substring(greetingKey.LastIndexOf('.') + 1);
            var message = GetRotatingMessage(greetingKey + "Msg", slot);
            
            // Explicitly set duration to 6 seconds as requested (longer than default)
            _notificationService.Show(title, message, NotificationType.Info, 12);
        }
        catch (Exception ex)
        {
            // Silently fail to ensure app stability as requested
            _loggingService.LogError("Error showing smart greeting", ex);
        }
    }

    private string GetRotatingMessage(string baseMessageKey, string slot)
    {
        var available = new System.Collections.Generic.List<string>();

        var baseValue = _languageService[baseMessageKey];
        if (!IsMissing(baseValue, baseMessageKey))
        {
            available.Add(baseValue);
        }

        for (int i = 1; i <= 20; i++)
        {
            var key = baseMessageKey + "." + i;
            var value = _languageService[key];
            if (!IsMissing(value, key))
            {
                available.Add(value);
            }
        }

        if (available.Count == 0)
        {
            return $"[{baseMessageKey}]";
        }

        int lastIndex = slot switch
        {
            "Morning" => _settingsService.LastGreetingIndexMorning,
            "Afternoon" => _settingsService.LastGreetingIndexAfternoon,
            "Evening" => _settingsService.LastGreetingIndexEvening,
            "Night" => _settingsService.LastGreetingIndexNight,
            _ => -1
        };

        var nextIndex = (lastIndex + 1) % available.Count;

        switch (slot)
        {
            case "Morning": _settingsService.LastGreetingIndexMorning = nextIndex; break;
            case "Afternoon": _settingsService.LastGreetingIndexAfternoon = nextIndex; break;
            case "Evening": _settingsService.LastGreetingIndexEvening = nextIndex; break;
            case "Night": _settingsService.LastGreetingIndexNight = nextIndex; break;
        }

        return available[nextIndex];
    }

    private static bool IsMissing(string resolvedValue, string key)
    {
        return string.Equals(resolvedValue, $"[{key}]", StringComparison.Ordinal);
    }
}
