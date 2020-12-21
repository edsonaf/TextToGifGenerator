using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Text2GifGenerator.Converters
{
    public class ImageToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var myImage = (Image) value;

            if (myImage == null)
            {
                return null;
            }

            // TODO: Remove this try catch
            try
            {
                var bitmap = new Bitmap(myImage);
                var bmpPt = bitmap.GetHbitmap();
                var bitmapSource =
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
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr value);
    }
}