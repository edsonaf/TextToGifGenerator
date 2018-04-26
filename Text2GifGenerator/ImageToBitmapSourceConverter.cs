using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Text2GifGenerator
{
  public class ImageToBitmapSourceConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Image myImage = (Image) value;

      if (myImage == null)
      {
        return null;
      }

      Bitmap bitmap = new Bitmap(myImage);
      IntPtr bmpPt = bitmap.GetHbitmap();
      BitmapSource bitmapSource =
        Imaging.CreateBitmapSourceFromHBitmap(
          bmpPt,
          IntPtr.Zero,
          Int32Rect.Empty,
          BitmapSizeOptions.FromEmptyOptions());

      //freeze bitmapSource and clear memory to avoid memory leaks
      bitmapSource.Freeze();
      DeleteObject(bmpPt);

      return bitmapSource;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject(IntPtr value);
  }
}