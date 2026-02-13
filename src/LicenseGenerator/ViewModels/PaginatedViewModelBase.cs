using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LicenseGenerator.ViewModels;

public abstract partial class PaginatedViewModelBase : ViewModelBase
{
    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _pageSize = 10;

    [ObservableProperty]
    private int _totalCount;

    public IEnumerable<int> PageSizeOptions { get; } = new[] { 5, 10, 15, 20, 30, 50 };

    [RelayCommand]
    protected virtual void NextPage()
    {
        if (CurrentPage * PageSize < TotalCount)
        {
            CurrentPage++;
            UpdateFilter();
        }
    }

    [RelayCommand]
    protected virtual void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            UpdateFilter();
        }
    }

    partial void OnPageSizeChanged(int value)
    {
        CurrentPage = 1;
        UpdateFilter();
    }

    protected abstract void UpdateFilter();
}
