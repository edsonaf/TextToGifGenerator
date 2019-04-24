using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Gif.Components;

namespace TextToGifGenerator
{
  public class TextToImageConverter : ITextToImageConverter
  {
    private Graphics _drawing;
    private SizeF _textSize;
    private Brush _textBrush;

    private Image _currentImage;
    private List<Image> _images;

    private int _amountOfImages; 
    
    public List<Image> DrawText(TextToImageSettings settings, string text)
    {
      _images = new List<Image>();

      // first, create a dummy bitmap just to get a graphics object
      _currentImage = new Bitmap(settings.MaxWidth, settings.MaxHeight);
      _drawing = Graphics.FromImage(_currentImage);

      // measure the string to see how big the image needs to be
      _textSize = _drawing.MeasureString(text, settings.Font);
      _textBrush = new SolidBrush(settings.Foreground);

      // free up the dummy image and old graphics object
      _currentImage.Dispose();
      _drawing.Dispose();

      int currentWidthLocation = settings.MaxWidth;

      CreateImages(settings, text, currentWidthLocation);

      return _images;
    }

    private void CreateImages(TextToImageSettings settings, string text, int currentWidthLocation)
    {
      CalculateAmountOfImages(settings);

      for (int i = _amountOfImages; i > 0; i--)
      {
        // create a new image of the right size
        _currentImage = new Bitmap(settings.MaxWidth, settings.MaxHeight);
        SetupNewDrawing();

        // set the background
        _drawing.Clear(settings.Background);

        DrawToImage(settings, text, currentWidthLocation);

        _images.Add(_currentImage);
        currentWidthLocation--;
      }
    }

    private void CalculateAmountOfImages(TextToImageSettings settings)
    {
      float amountOfImage = settings.Loop ? 
        (settings.MaxWidth + _textSize.Width > _textSize.Width * 2 ? settings.MaxWidth + _textSize.Width : _textSize.Width * 2) : 
        (settings.MaxWidth > _textSize.Width ? settings.MaxWidth : _textSize.Width);

      _amountOfImages = (int) amountOfImage;
    }
    
    private void SetupNewDrawing()
    {
      _drawing = Graphics.FromImage(_currentImage);
      _drawing.CompositingQuality = CompositingQuality.HighQuality;
      _drawing.InterpolationMode = InterpolationMode.Bicubic;
      _drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
      _drawing.SmoothingMode = SmoothingMode.HighQuality;
      _drawing.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
    }
    
    private void DrawToImage(TextToImageSettings settings, string text, int currentWidthLocation)
    {
      RectangleF layoutRectangle = new RectangleF(currentWidthLocation, 10, _textSize.Width, settings.MaxHeight);
      _drawing.DrawString(text, settings.Font, _textBrush, layoutRectangle, settings.StringFormat);
      _drawing.Save();
      _drawing.Dispose();
    }
   
    public void CreateGif(List<Image> images, string filePath, bool repeat = true)
    {
      /* create Gif */

      // TODO: Replace Filepath
      string outputFilePath = filePath;
      AnimatedGifEncoder e = new AnimatedGifEncoder();
      e.Start(outputFilePath);
      e.SetDelay(images.Count / 50);

      // -1: no repeat, 
      //  0: always repeat (loop)
      e.SetRepeat(repeat ? 0 : -1);

      for (int i = 0, count = images.Count; i < count; i++)
      {
        e.AddFrame(images[i]);
      }

      e.Finish();
    }

    public void ExtractGif(string filePath)
    {
      string outputPath = "d:\\";
      GifDecoder gifDecoder = new GifDecoder();
      
      gifDecoder.Read("d:\\test.gif");
      for (int i = 0, count = gifDecoder.GetFrameCount(); i < count; i++)
      {
        Image frame = gifDecoder.GetFrame(i);  // frame i
        frame.Save(outputPath + Guid.NewGuid().ToString() + ".png", ImageFormat.Png);
      }
    }
  }
}