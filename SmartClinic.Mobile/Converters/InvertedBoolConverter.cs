using System.Globalization;

namespace SmartClinic.Mobile.Converters;

public class InvertedBoolConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        // Reverse a boolean value for controls that display when a state is false.
        return value is bool booleanValue && !booleanValue;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        // Support reverse conversion if the converter is used with two-way binding.
        return value is bool booleanValue && !booleanValue;
    }
}