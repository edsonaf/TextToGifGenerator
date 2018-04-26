using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Text2GifGenerator.Converters
{
  public class ArcSizeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double d && d > 0.0)
      {
        return new Size(d / 2, d / 2);
      }

      return new Point();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    }
  }
}