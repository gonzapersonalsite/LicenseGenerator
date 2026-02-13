using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace LicenseGenerator.Converters;

public class NullableDateToTextConverter : IValueConverter
{
    private string GetPermanenteText()
    {
        if (Application.Current != null && 
            Application.Current.Resources.TryGetResource("HistoryPermanente", null, out var res) && 
            res is string text)
        {
            return text;
        }
        return "Permanente";
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return GetPermanenteText();

        if (value is DateTime dt)
        {
            if (dt == DateTime.MinValue || dt.Year <= 1)
                return GetPermanenteText();
            
            return dt.ToString("dd/MM/yyyy");
        }
        
        if (value is DateTimeOffset dto)
        {
            if (dto == DateTimeOffset.MinValue || dto.Year <= 1)
                return GetPermanenteText();
                
            return dto.ToString("dd/MM/yyyy");
        }

        return GetPermanenteText();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
