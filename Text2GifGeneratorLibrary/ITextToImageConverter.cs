using System.Collections.Generic;
using System.Drawing;

namespace TextToGifGenerator
{
  public interface ITextToImageConverter
  {
    /// <summary>
    /// https://stackoverflow.com/questions/2070365/how-to-generate-an-image-from-text-on-fly-at-runtime 
    /// Link above contains code to create one image from a text. Code has been extended to create multiple images and place them in a list
    /// </summary>
    /// <param name="text"></param>
    /// <param name="font"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    /// <returns></returns>
    List<Image> DrawText(string text, Font font, int maxWidth, int maxHeight);


    /// <summary>
    /// https://www.codeproject.com/Articles/11505/NGif-Animated-GIF-Encoder-for-NET
    /// Using project in link above we create a gif from list of images we made with above function.
    /// </summary>
    /// <param name="images"></param>
    void CreateGif(List<Image> images);

  }
}