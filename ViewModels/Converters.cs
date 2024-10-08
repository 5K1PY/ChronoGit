using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace ChronoGit.ViewModels;

public class StringToIntConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is int intValue) {
            return (intValue == -1) ? "" : intValue.ToString();
        }
        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (int.TryParse(value?.ToString(), out int res)) {
            return res;
        }
        return -1;
    }
}

public class IntPlusOneConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is int intValue) {
            return (intValue + 1).ToString();
        }
        return "0";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is string stringValue && int.TryParse(stringValue, out int intValue)) {
            return intValue - 1;
        }
        return -1;
    }
}

public class BoolPlusOneConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is bool boolValue) {
            return boolValue ? 2 : 1;
        }
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is int intValue) {
            return intValue == 2;
        }
        return false;
    }
}

public class SelectionToColorConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
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

public class ValidityToColorConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is bool boolValue) {
            return boolValue ? Brushes.Black : Brushes.Red;
        }
        // Throwing here doesn't work anyway
        return Brushes.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

