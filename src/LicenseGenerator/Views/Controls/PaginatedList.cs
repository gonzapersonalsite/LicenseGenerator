using Avalonia;
using Avalonia.Controls;
using System.Windows.Input;

namespace LicenseGenerator.Views.Controls;

public class PaginatedList : ContentControl
{
    public static readonly StyledProperty<ICommand> NextPageCommandProperty =
        AvaloniaProperty.Register<PaginatedList, ICommand>(nameof(NextPageCommand));

    public ICommand NextPageCommand
    {
        get => GetValue(NextPageCommandProperty);
        set => SetValue(NextPageCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> PreviousPageCommandProperty =
        AvaloniaProperty.Register<PaginatedList, ICommand>(nameof(PreviousPageCommand));

    public ICommand PreviousPageCommand
    {
        get => GetValue(PreviousPageCommandProperty);
        set => SetValue(PreviousPageCommandProperty, value);
    }

    public static readonly StyledProperty<int> CurrentPageProperty =
        AvaloniaProperty.Register<PaginatedList, int>(nameof(CurrentPage));

    public int CurrentPage
    {
        get => GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }

    public static readonly StyledProperty<int> TotalCountProperty =
        AvaloniaProperty.Register<PaginatedList, int>(nameof(TotalCount));

    public int TotalCount
    {
        get => GetValue(TotalCountProperty);
        set => SetValue(TotalCountProperty, value);
    }

    public static readonly StyledProperty<int> PageSizeProperty =
        AvaloniaProperty.Register<PaginatedList, int>(nameof(PageSize), 10, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public int PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }

    public static readonly StyledProperty<System.Collections.IEnumerable> PageSizeOptionsProperty =
        AvaloniaProperty.Register<PaginatedList, System.Collections.IEnumerable>(nameof(PageSizeOptions), new int[] { 5, 10, 15, 20, 30, 50 });

    public System.Collections.IEnumerable PageSizeOptions
    {
        get => GetValue(PageSizeOptionsProperty);
        set => SetValue(PageSizeOptionsProperty, value);
    }
    
    // Read-only property for TotalPages
    public static readonly DirectProperty<PaginatedList, int> TotalPagesProperty =
        AvaloniaProperty.RegisterDirect<PaginatedList, int>(
            nameof(TotalPages),
            o => o.TotalPages);

    private int _totalPages;
    public int TotalPages
    {
        get => _totalPages;
        private set => SetAndRaise(TotalPagesProperty, ref _totalPages, value);
    }

    // Read-only property for DisplayCurrentPage (handles 0 items case)
    public static readonly DirectProperty<PaginatedList, int> DisplayCurrentPageProperty =
        AvaloniaProperty.RegisterDirect<PaginatedList, int>(
            nameof(DisplayCurrentPage),
            o => o.DisplayCurrentPage);

    public int DisplayCurrentPage => TotalCount == 0 ? 0 : CurrentPage;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TotalCountProperty || change.Property == PageSizeProperty)
        {
            UpdateTotalPages();
            RaisePropertyChanged(DisplayCurrentPageProperty, default, DisplayCurrentPage);
        }
        
        if (change.Property == CurrentPageProperty)
        {
            RaisePropertyChanged(DisplayCurrentPageProperty, default, DisplayCurrentPage);
        }
    }

    private void UpdateTotalPages()
    {
        int pageSize = PageSize > 0 ? PageSize : 1;
        TotalPages = (TotalCount + pageSize - 1) / pageSize;
    }
}
