using LicenseGenerator.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LicenseGenerator.ViewModels;

public class NavigationItem
{
    public required string Label { get; set; }
    public string? IconKey { get; set; }
    public Type? ViewModelType { get; set; }
    public bool IsHeader { get; set; }
}

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    
    public static MainWindowViewModel? Instance { get; private set; }

    [ObservableProperty]
    private ILanguageService _languageService;

    public INotificationService NotificationService { get; }

    [ObservableProperty]
    private ViewModelBase _currentPage = default!;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private NavigationItem? _selectedMainItem;

    public ObservableCollection<NavigationItem> MenuItems { get; } = new();

    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Instance = this;
        _languageService = _serviceProvider.GetRequiredService<ILanguageService>();
        NotificationService = _serviceProvider.GetRequiredService<INotificationService>();

        // Group: General
        AddMenuHeader("NavGeneral");
        AddMenuItem(new NavigationItem 
        { 
            Label = "NavDashboard", 
            IconKey = "HomeRegular", 
            ViewModelType = typeof(DashboardViewModel) 
        });

        // Group: Aplicaciones
        AddMenuHeader("NavApps");
        AddMenuItem(new NavigationItem 
        { 
            Label = "NavManageApps", 
            IconKey = "AppsRegular", 
            ViewModelType = typeof(AppsViewModel) 
        });

        // Group: Licencias
        AddMenuHeader("NavLicenses");
        AddMenuItem(new NavigationItem 
        { 
            Label = "NavGenerate", 
            IconKey = "KeyRegular", 
            ViewModelType = typeof(GenerateLicenseViewModel) 
        });
        AddMenuItem(new NavigationItem 
        { 
            Label = "NavHistory", 
            IconKey = "HistoryRegular", 
            ViewModelType = typeof(HistoryViewModel) 
        });

        // Group: Configuración
        AddMenuHeader("NavConfig");
        AddMenuItem(new NavigationItem 
        { 
            Label = "NavSettings", 
            IconKey = "SettingsRegular", 
            ViewModelType = typeof(SettingsViewModel) 
        });

        // Select first selectable item by default
        SelectedMainItem = MenuItems.FirstOrDefault(i => !i.IsHeader);
    }

    private void AddMenuHeader(string title)
    {
        MenuItems.Add(new NavigationItem 
        { 
            Label = title, 
            IsHeader = true 
        });
    }

    private void AddMenuItem(NavigationItem item)
    {
        MenuItems.Add(item);
    }

    partial void OnSelectedMainItemChanged(NavigationItem? value)
    {
        if (value is not null && !value.IsHeader && value.ViewModelType != null)
        {
            NavigateTo(value);
        }
    }

    public void NavigateTo(NavigationItem item)
    {
        if (item.ViewModelType == null) return;

        var vm = _serviceProvider.GetRequiredService(item.ViewModelType);
        if (vm is ViewModelBase vmb)
        {
            CurrentPage = vmb;
        }
    }

    [RelayCommand]
    public void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand]
    public void NavigateToApps()
    {
        var item = MenuItems.FirstOrDefault(i => i.ViewModelType == typeof(AppsViewModel));
        if (item != null)
        {
            NavigateTo(item);
        }
    }

    [RelayCommand]
    public void NavigateToHistory()
    {
        var item = MenuItems.FirstOrDefault(i => i.ViewModelType == typeof(HistoryViewModel));
        if (item != null)
        {
            NavigateTo(item);
        }
    }
}
