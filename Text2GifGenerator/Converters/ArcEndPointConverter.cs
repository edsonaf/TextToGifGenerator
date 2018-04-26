using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Text2GifGenerator.Converters
{
  public class ArcEndPointConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      double actualWidth = values[0].ExtractDouble();
      double value = values[1].ExtractDouble();
      double minimum = values[2].ExtractDouble();
      double maximum = values[3].ExtractDouble();

      if (new[] {actualWidth, value, minimum, maximum}.AnyNan())
      {
        return Binding.DoNothing;
      }

      if (values.Length == 5)
      {
        double fullIndeterminateScaling = values[4].ExtractDouble();
        if (!double.IsNaN(fullIndeterminateScaling) && fullIndeterminateScaling > 0.0)
        {
          value = (maximum - minimum) * fullIndeterminateScaling;
        }
      }

      double percent = maximum <= minimum ? 1.0 : (value - minimum) / (maximum - minimum);
      double degrees = 360 * percent;
      double radians = degrees * (Math.PI / 180);

      Point centre = new Point(actualWidth / 2, actualWidth / 2);
      double hypotenuseRadius = actualWidth / 2;

      double adjacent = Math.Cos(radians) * hypotenuseRadius;
      double opposite = Math.Sin(radians) * hypotenuseRadius;

      return new Point(centre.X + opposite, centre.Y - adjacent);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal static class LocalEx
  {
    public static double ExtractDouble(this object val)
    {
      double d = val as double? ?? double.NaN;
      return double.IsInfinity(d) ? double.NaN : d;
    }


    public static bool AnyNan(this IEnumerable<double> vals)
    {
      return vals.Any(double.IsNaN);
    }
  }
}