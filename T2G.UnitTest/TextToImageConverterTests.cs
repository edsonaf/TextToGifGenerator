using System.Drawing;
using Font = System.Drawing.Font;

namespace T2G.UnitTest
{
    public class Tests
    {
        public class TextToImageConverterTests
        {
            [Test]
            public void DrawText_LoopIsFalse_GenerateImages()
            {
                var converter = new TextToImageConverter();
                var settings = new TextToImageSettings()
                {
                    Font = new Font(new FontFamily("Tahoma"), 20, FontStyle.Bold),
                    Loop = false
                };

                var textImages = converter.DrawText(settings, "Hello World");

                Assert.IsTrue(textImages.Count > 0);
            }

            [Test]
            public void DrawText_LoopIsTrue_GenerateImages()
            {
                var converter = new TextToImageConverter();
                var settings = new TextToImageSettings()
                {
                    Font = new Font(new FontFamily("Tahoma"), 20, FontStyle.Bold),
                    Loop = true
                };

                var textImages = converter.DrawText(settings, "Hello World");

                Assert.IsTrue(textImages.Count > 0);
            }

            [Test]
            public void DrawText_InvertBackgroundAndForeground_GenerateImages()
            {
                var converter = new TextToImageConverter();
                var settings = new TextToImageSettings
                {
                    Font = new Font(new FontFamily("Tahoma"), 20, FontStyle.Bold),
                    MaxHeight = 5678,
                    MaxWidth = 234,
                    Foreground = Color.Black,
                    Background = Color.Wheat
                };

                var textImages = converter.DrawText(settings, "Hello World");

                Assert.IsTrue(textImages.Count > 0);
            }

            [Test]
            public void CreateGif_DefaultHelloWorld_ExpectedSameAsResource()
            {
                var converter = new TextToImageConverter();
                var settings = new TextToImageSettings();
                var currentDirectory = Environment.CurrentDirectory;
                    
                var images = converter.DrawText(settings, "Hello World");
                converter.CreateGif(images, $"{currentDirectory}\\test2.gif", null);
                var resourceDirectory = $"{currentDirectory}..\\..\\..\\..\\..\\resources\\hello_world_default.gif";
                var expected = File.Open(resourceDirectory, FileMode.Open);
                var actual = File.Open(currentDirectory + "\\test.gif", FileMode.Open);
                Assert.AreEqual(expected, actual);
            }

            [TearDown]
            public void DeleteAllGif()
            {
                var currentDirectory = Environment.CurrentDirectory;
            }
        }
    }
}