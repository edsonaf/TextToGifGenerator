using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Text2GifGeneratorLibrary;

namespace TextToGifGenerator.Cmd
{
    static class Program
    {
        private static ITextToImageConverter _converter;

        private static async Task Main(string[] args)
        {
            var text = args.Length > 0 ? args[0] : "Hello World";
            var stopWatch = new Stopwatch();

            _converter = new TextToImageConverter();
            
            Console.WriteLine($"{stopWatch.ElapsedMilliseconds}--- Start Drawing of text {text}.");
            stopWatch.Start();

            var drawText = _converter.DrawText(T2IHelper.Default, text);
            Console.WriteLine($"{stopWatch.ElapsedMilliseconds}--- Done.");
            Console.WriteLine($"{stopWatch.ElapsedMilliseconds}--- Start gif generation");
            await _converter.CreateGif(drawText, Directory.GetCurrentDirectory() + Guid.NewGuid() + ".gif");
            
            stopWatch.Stop();
            
            Console.WriteLine($"{stopWatch.ElapsedMilliseconds}--- Done");
        }

        // private static void SaveInTempFolder(List<Image> drawText)
        // {
        //     var tempPath = Directory.GetCurrentDirectory();
        //     foreach (var image in drawText)
        //     {
        //         image.Save(tempPath + Guid.NewGuid() + ".png");
        //     }
        //     
        //     Process.Start("explorer.exe", tempPath);
        // }
    }
}