using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Gif.Components;

namespace TextToGifGenerator
{
  public class TextToImageConverter
  {
    public List<Image> DrawText(TextToImageSettings settings, string text)
    {
      List<Image> images = new List<Image>();

      // first, create a dummy bitmap just to get a graphics object
      Bitmap img = new Bitmap(settings.MaxWidth, settings.MaxHeight);
      Graphics drawing = Graphics.FromImage(img);

      // measure the string to see how big the image needs to be
      SizeF textSize = drawing.MeasureString(text, settings.Font);

      // free up the dummy image and old graphics object
      img.Dispose();
      drawing.Dispose();

      int currentWidthLocation = settings.MaxWidth;

      float amountOfImage;
      if (settings.Loop)
      {
        amountOfImage = settings.MaxWidth + textSize.Width > textSize.Width * 2 ? settings.MaxWidth + textSize.Width : textSize.Width * 2;
      }
      else
      {
        amountOfImage = settings.MaxWidth > textSize.Width ? settings.MaxWidth : textSize.Width;
      }

      for (int i = (int) amountOfImage; i > 0; i--)
      {
        // create a new image of the right size
        img = new Bitmap(settings.MaxWidth, settings.MaxHeight);
        drawing = Graphics.FromImage(img);

        // Adjust for high quality
        drawing.CompositingQuality = CompositingQuality.HighQuality;
        drawing.InterpolationMode = InterpolationMode.Bicubic;
        drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
        drawing.SmoothingMode = SmoothingMode.HighQuality;
        drawing.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        // paint the background
        drawing.Clear(settings.Background);

        // create a brush for the text
        Brush textBrush = new SolidBrush(settings.Foreground);

        drawing.DrawString(text, settings.Font, textBrush, new RectangleF(currentWidthLocation, 10, textSize.Width, settings.MaxHeight), settings.StringFormat);
        drawing.Save();

        textBrush.Dispose();
        drawing.Dispose();

        images.Add(img);
        currentWidthLocation--;
      }

      return images;
    }

    public void CreateGif(List<Image> images, string filePath)
    {
      /* create Gif */

      // you should replace filepath
      string outputFilePath = $"d:\\test.gif";
      AnimatedGifEncoder e = new AnimatedGifEncoder();
      e.Start(outputFilePath);
      e.SetDelay(images.Count / 50);

      // -1: no repeat, 
      //  0: always repeat (loop)
      e.SetRepeat(0);

      for (int i = 0, count = images.Count; i < count; i++)
      {
        e.AddFrame(images[i]);
      }

      e.Finish();

      /* extract Gif */
      //string outputPath = "d:\\";
      //GifDecoder gifDecoder = new GifDecoder();
      //gifDecoder.Read("d:\\test.gif");
      //for (int i = 0, count = gifDecoder.GetFrameCount(); i < count; i++)
      //{
      //  Image frame = gifDecoder.GetFrame(i);  // frame i
      //  frame.Save(outputPath + Guid.NewGuid().ToString() + ".png", ImageFormat.Png);
      //}
    }
  }

  public class TextToImageSettings
  {
    public Font Font { get; set; }
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    public bool Loop { get; set; }

    public Color Foreground { get; set; } = Color.White;
    public Color Background { get; set; } = Color.Black;

    public StringFormat StringFormat { get; set; }

    public TextToImageSettings()
    {
      // Set Default Values
      
      //set the stringformat flags to rtl
      StringFormat = new StringFormat
      {
        // uncomment the next line for right to left languages
        FormatFlags = StringFormatFlags.DirectionRightToLeft,
        Trimming = StringTrimming.Word
      };      
    }
  }
}