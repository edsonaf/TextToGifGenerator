using System.Drawing;

namespace TextToGifGenerator
{
  public class TextToImageSettings
  {
    public Font Font { get; set; }         = new Font(new FontFamily("Tahoma").Name, 12);
    public int MaxWidth { get; set; }      = 128;
    public int MaxHeight { get; set; }     = 36;
    public bool Loop { get; set; }         = false;
    public Color Foreground { get; set; }  = Color.White;
    public Color Background { get; set; }  = Color.Black;

    public StringFormat StringFormat { get; set; }

    public TextToImageSettings()
    {
      StringFormat = new StringFormat
      {
        // uncomment the next line for right to left languages
        //FormatFlags = StringFormatFlags.DirectionRightToLeft,
        Trimming = StringTrimming.Word
      };
    }
  }
}