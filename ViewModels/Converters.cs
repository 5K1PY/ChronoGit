using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace ChronoGit.ViewModels;

public class SelectionToColorConverter : IValueConverter {
    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is bool boolValue) {
            return boolValue ? Brushes.Wheat : Brushes.White;
        }
        // Throwing here doesn't work anyway
        return Brushes.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

