using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Gif.Components;

namespace TextToGifGenerator
{
  public class TextToImageConverter : ITextToImageConverter
  {
    public List<Image> DrawText(string text, Font font, int maxWidth, int maxHeight)
    {
      List<Image> images = new List<Image>();

      // first, create a dummy bitmap just to get a graphics object
      Bitmap img = new Bitmap(maxWidth, maxHeight);
      Graphics drawing = Graphics.FromImage(img);

      // measure the string to see how big the image needs to be
      SizeF textSize = drawing.MeasureString(text, font);

      //set the stringformat flags to rtl
      StringFormat sf = new StringFormat
      {
        // uncomment the next line for right to left languages
        //FormatFlags = StringFormatFlags.DirectionRightToLeft,
        Trimming = StringTrimming.Word
      };

      

      // free up the dummy image and old graphics object
      img.Dispose();
      drawing.Dispose();

      var currentWidthLocation = maxWidth;
      var amountOfImage = maxWidth > textSize.Width ? maxWidth : textSize.Width;

      for (var i = (int)amountOfImage; i > 0; i--)
      {
        // create a new image of the right size
        img = new Bitmap(maxWidth, maxHeight);
        drawing = Graphics.FromImage(img);
        
        // Adjust for high quality
        drawing.CompositingQuality = CompositingQuality.HighQuality;
        drawing.InterpolationMode = InterpolationMode.HighQualityBilinear;
        drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
        drawing.SmoothingMode = SmoothingMode.HighQuality;
        drawing.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        // paint the background
        drawing.Clear(Color.Black);

        // create a brush for the text
        Brush textBrush = new SolidBrush(Color.White);
        
        drawing.DrawString(text, font, textBrush, new RectangleF(currentWidthLocation, 10, textSize.Width, maxHeight), sf);
        drawing.Save();

        textBrush.Dispose();
        drawing.Dispose();

        images.Add(img);
        currentWidthLocation--;
      }


      //img.Save(path, ImageFormat.Png);
      //img.Dispose();

      return images;
    }

    public void CreateGif(List<Image> images)
    {
      /* create Gif */
      
      // you should replace filepath
      String outputFilePath = $"d:\\test.gif";
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
}
