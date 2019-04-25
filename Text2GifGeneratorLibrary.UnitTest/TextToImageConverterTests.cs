using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using TextToGifGenerator;

namespace Text2GifGeneratorLibrary.UnitTest
{
  [TestFixture]
  public class TextToImageConverterTests
  {
   [Test]
    public void DrawText_LoopIsFalse_GenerateImages()
    {
      TextToImageConverter converter = new TextToImageConverter();
      TextToImageSettings settings = new TextToImageSettings()
      {
        Font = new Font(new FontFamily("Tahoma"), 20, FontStyle.Bold),
        MaxHeight = 999,
        MaxWidth = 999,
        Loop = false
      };

      List<Image> textImages = converter.DrawText(settings, "Hello World");
      
      Assert.IsTrue(textImages.Count > 0);
    }
    
    [Test]
    public void DrawText_LoopIsTrue_GenerateImages()
    {
      TextToImageConverter converter = new TextToImageConverter();
      TextToImageSettings settings = new TextToImageSettings()
      {
        Font = new Font(new FontFamily("Tahoma"), 20, FontStyle.Bold),
        MaxHeight = 999,
        MaxWidth = 999,
        Loop = true
      };

      List<Image> textImages = converter.DrawText(settings, "Hello World");
      
      Assert.IsTrue(textImages.Count > 0);
    }

    [Test]
    public void DrawText_InvertBackgroundAndForeground_GenerateImages()
    {
      TextToImageConverter converter = new TextToImageConverter();
      TextToImageSettings settings = new TextToImageSettings
      {
        Font = new Font(new FontFamily("Tahoma"), 20, FontStyle.Bold),
        MaxHeight = 5678,
        MaxWidth = 234,
        Foreground = Color.Black,
        Background = Color.Wheat
      };

      List<Image> textImages = converter.DrawText(settings, "Hello World");

      Assert.IsTrue(textImages.Count > 0);
    }
  }
}