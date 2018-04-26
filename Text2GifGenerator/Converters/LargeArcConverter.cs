using System;
using System.Globalization;
using System.Windows.Data;

namespace Text2GifGenerator.Converters
{
  public class LargeArcConverter : IMultiValueConverter
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

      if (values.Length == 4)
      {
        double fullIndeterminateScaling = values[3].ExtractDouble();
        if (!double.IsNaN(fullIndeterminateScaling) && fullIndeterminateScaling > 0.0)
        {
          value = (maximum - minimum) * fullIndeterminateScaling;
        }
      }

      double percent = maximum <= minimum ? 1.0 : (value - minimum) / (maximum - minimum);

      return percent > 0.5;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}