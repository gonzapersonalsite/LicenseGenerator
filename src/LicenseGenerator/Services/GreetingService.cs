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
            var message = _languageService[greetingKey + "Msg"];
            
            // Explicitly set duration to 6 seconds as requested (longer than default)
            _notificationService.Show(title, message, NotificationType.Info, 12);
        }
        catch (Exception ex)
        {
            // Silently fail to ensure app stability as requested
            _loggingService.LogError("Error showing smart greeting", ex);
        }
    }
}
