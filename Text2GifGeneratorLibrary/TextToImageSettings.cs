using System.Drawing;

namespace Text2GifGeneratorLibrary
{
    public class TextToImageSettings
    {
        public Font Font { get; set; } = new Font(new FontFamily("Tahoma").Name, 12);
        public int MaxWidth { get; set; } = 128;
        public int MaxHeight { get; set; } = 36;
        public bool Loop { get; set; }
        public Color Foreground { get; set; } = Color.White;
        public Color Background { get; set; } = Color.Black;
        public LibraryEnums.FLowDirection FlowDirection { get; set; } = LibraryEnums.FLowDirection.UpToDown;
        public StringFormat StringFormat { get; }

        public TextToImageSettings()
        {
            StringFormat = new StringFormat
            {
                // TODO: When introducing language detection:
                // uncomment the next line for right to left languages
                //FormatFlags = StringFormatFlags.DirectionRightToLeft,
                // Trimming = StringTrimming.Word
            };
        }
    }
}