﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Text2GifGenerator.Converters
{
  public class RotateTransformCentreConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      //value == actual width
      return (double) value / 2;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    }
  }
}