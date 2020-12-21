using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Threading.Tasks;
using Components;

namespace Text2GifGeneratorLibrary
{
    public class TextToImageConverter : ITextToImageConverter
    {
        private SizeF _textSize;

        public List<Image> DrawText(TextToImageSettings settings, string text)
        {
            // first, create a dummy bitmap just to get a graphics object
            var currentImage = new Bitmap(settings.MaxWidth, settings.MaxHeight);
            var drawing = Graphics.FromImage(currentImage);

            // measure the string to see how big the image needs to be
            _textSize = drawing.MeasureString(text, settings.Font);

            // free up the dummy image and old graphics object
            currentImage.Dispose();
            drawing.Dispose();

            var currentWidthLocation = settings.MaxWidth;
            var currentHeightLocation = settings.MaxHeight;

            var images = CreateImages(settings, text, currentWidthLocation, currentHeightLocation);
            return images;
        }

        private List<Image> CreateImages(TextToImageSettings settings, string text, int currentWidthLocation,
            int currentHeightLocation)
        {
            var images = new List<Image>();
            var amountOfImages = CalculateAmountOfImages(settings);

            for (var i = amountOfImages; i > 0; i--)
            {
                // create a new image of the right size
                var currentImage = new Bitmap(settings.MaxWidth, settings.MaxHeight);
                var drawing = SetDefaultDrawingProperties(currentImage);

                // set the background
                drawing.Clear(settings.Background);

                if (settings.FlowDirection == LibraryEnums.FLowDirection.LeftToRight ||
                    settings.FlowDirection == LibraryEnums.FLowDirection.RightToLeft) // Horizontal scroll
                {
                    DrawToImageHorizontal(settings, text, currentWidthLocation, drawing);
                    images.Add(currentImage);
                    currentWidthLocation--;
                }
                else // vertical scroll
                {
                    DrawToImageVertical(settings, text, currentHeightLocation, drawing);
                    images.Add(currentImage);
                    currentHeightLocation--;
                }
            }

            if (settings.FlowDirection == LibraryEnums.FLowDirection.LeftToRight ||
                settings.FlowDirection == LibraryEnums.FLowDirection.UpToDown)
            {
                images.Reverse();
            }

            return images;
        }

        private int CalculateAmountOfImages(TextToImageSettings settings)
        {
            float amountOfImage;
            if (settings.FlowDirection == LibraryEnums.FLowDirection.LeftToRight ||
                settings.FlowDirection == LibraryEnums.FLowDirection.RightToLeft)
            {
                amountOfImage = settings.Loop
                    ? settings.MaxWidth + _textSize.Width > _textSize.Width * 2
                        ? settings.MaxWidth + _textSize.Width
                        : _textSize.Width * 2
                    : settings.MaxWidth > _textSize.Width
                        ? settings.MaxWidth
                        : _textSize.Width;
            }
            else
            {
                amountOfImage = settings.Loop ? settings.MaxHeight + _textSize.Height > _textSize.Height * 2
                        ? settings.MaxHeight + _textSize.Height
                        : _textSize.Height * 2
                    : settings.MaxHeight > _textSize.Height ? settings.MaxHeight : _textSize.Height;
            }

            return (int) amountOfImage;
        }

        private static Graphics SetDefaultDrawingProperties(Image currentImage)
        {
            var drawing = Graphics.FromImage(currentImage);
            drawing.CompositingQuality = CompositingQuality.HighQuality;
            drawing.InterpolationMode = InterpolationMode.Bicubic;
            drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
            drawing.SmoothingMode = SmoothingMode.HighQuality;
            drawing.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            return drawing;
        }

        private void DrawToImageHorizontal(TextToImageSettings settings, string text, int currentWidthLocation,
            Graphics drawing)
        {
            var layoutRectangle = new RectangleF(currentWidthLocation, 10, _textSize.Width, settings.MaxHeight);

            drawing.DrawString(text, settings.Font, new SolidBrush(settings.Foreground), layoutRectangle,
                settings.StringFormat);
            drawing.Save();
            drawing.Dispose();
        }

        private void DrawToImageVertical(TextToImageSettings settings, string text, int currentHeightLocation,
            Graphics drawing)
        {
            var layoutRectangle = new RectangleF(10, currentHeightLocation, _textSize.Width, settings.MaxHeight);

            drawing.DrawString(text, settings.Font, new SolidBrush(settings.Foreground), layoutRectangle,
                settings.StringFormat);
            drawing.Save();
            drawing.Dispose();
        }

        public async Task<bool> CreateGif(List<Image> images, string filePath, IProgress<ProgressReport> progress,
            bool repeat = true)
        {
            var ok = false;
            var outputFilePath = filePath;
            var e = new AnimatedGifEncoder();
            e.Start(outputFilePath);
            e.SetFrameRate(90); // 90 frames per second TODO: Should this be a setting?
            e.SetRepeat(repeat ? 0 : 1);
            await Task.Run(() =>
            {
                for (int i = 0, count = images.Count; i < count; i++)
                {
                    e.AddFrame(images[i]);
                    progress.Report(
                        new ProgressReport {CurrentProgressAmount = i, TotalProgressAmount = images.Count});
                }

                ok = e.Finish();
            });

            return ok;
        }

        public Task<bool> GenerateGif(TextToImageSettings settings, string text, string filePath,
            IProgress<ProgressReport> progress)
        {
            return CreateGif(DrawText(settings, text), filePath, progress, settings.Loop);
        }

        public List<Image> ExtractGif(string filePath)
        {
            var frameList = new List<Image>();
            var gifDecoder = new GifDecoder();

            gifDecoder.Read(filePath);
            for (int i = 0, count = gifDecoder.GetFrameCount(); i < count; i++)
            {
                var frame = gifDecoder.GetFrame(i);
                frameList.Add(frame);
                // const string outputPath = "d:\\";
                // frame.Save($"{outputPath + Guid.NewGuid()}.png", ImageFormat.Png);
            }

            return frameList;
        }
    }

    public class ProgressReport
    {
        //current progress
        public int CurrentProgressAmount { get; set; }

        //total progress
        public int TotalProgressAmount { get; set; }
    }
}