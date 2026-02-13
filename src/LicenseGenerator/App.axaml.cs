using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using LicenseGenerator.ViewModels;
using LicenseGenerator.Views;
using LicenseGenerator.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LicenseGenerator;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
            
            
            var settings = Services.GetRequiredService<ISettingsService>();
            var language = Services.GetRequiredService<ILanguageService>();
            
            language.SetLanguage(settings.CurrentLanguage);
            ApplyInitialTheme();
            ApplyFontSize(settings.FontSizeScaling);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ILanguageService, LanguageService>();
        services.AddSingleton<ILicenseGeneratorService, LicenseGeneratorService>();
        services.AddSingleton<IDataService, DataService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INotificationService, NotificationService>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<AppsViewModel>();
        services.AddTransient<GenerateLicenseViewModel>();
        services.AddTransient<HistoryViewModel>();
        services.AddTransient<SettingsViewModel>();
    }

    public void ApplyFontSize(double scaling)
    {
        if (Application.Current != null)
        {
            // Base font sizes as defined in App.axaml
            double baseTiny = 10;
            double baseSmall = 12;
            double baseNormal = 14;
            double baseMedium = 16;
            double baseLarge = 18;
            double baseExtraLarge = 24;
            double baseHuge = 28;
            double baseExtraHuge = 36;

            Application.Current.Resources["FontSizeTiny"] = baseTiny * scaling;
            Application.Current.Resources["FontSizeSmall"] = baseSmall * scaling;
            Application.Current.Resources["FontSizeNormal"] = baseNormal * scaling;
            Application.Current.Resources["FontSizeMedium"] = baseMedium * scaling;
            Application.Current.Resources["FontSizeLarge"] = baseLarge * scaling;
            Application.Current.Resources["FontSizeExtraLarge"] = baseExtraLarge * scaling;
            Application.Current.Resources["FontSizeHuge"] = baseHuge * scaling;
            Application.Current.Resources["FontSizeExtraHuge"] = baseExtraHuge * scaling;
        }
    }

    public void ApplyInitialTheme()
    {
        var settings = Services?.GetService<ISettingsService>();
        if (settings != null && Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = settings.AppTheme switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                "System" => ThemeVariant.Default,
                _ => ThemeVariant.Dark
            };
        }
    }
}
