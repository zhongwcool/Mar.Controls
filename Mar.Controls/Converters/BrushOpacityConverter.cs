using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Mar.Controls.Converters;

/// <summary>
/// BrushOpacityConverter is a converter that can be used to change the opacity of a brush.
/// </summary>
public class BrushOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            var opacity = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            SolidColorBrush rv = new(brush.Color)
            {
                Opacity = opacity
            };
            rv.Freeze();
            return rv;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}