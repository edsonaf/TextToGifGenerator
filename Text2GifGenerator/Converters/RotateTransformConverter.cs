using System;
using System.Globalization;
using System.Windows.Data;

namespace Text2GifGenerator.Converters
{
  public class RotateTransformConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      double value = values[0].ExtractDouble();
      double minimum = values[1].ExtractDouble();
      double maximum = values[2].ExtractDouble();

      if (new[] {value, minimum, maximum}.AnyNan())
      {
        return Binding.DoNothing;
      }

      double percent = maximum <= minimum ? 1.0 : (value - minimum) / (maximum - minimum);

      return 360 * percent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}