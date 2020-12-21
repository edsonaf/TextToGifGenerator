using System.Drawing;
using GalaSoft.MvvmLight;

namespace Text2GifGenerator
{
    public class AcceptableQualityOptions : ObservableObject
    {
        private int _gifHeight;

        private int _gifWidth;

        private string _name = "Undefined";

        private FontFamily _selectedFont = new FontFamily("Tahoma");

        private int _selectedFontSize;

        public AcceptableQualityOptions(string name, FontFamily fontFamily, int fontSize, int gifWidth, int gifHeight, bool loop)
        {
            Name = name;
            SelectedFont = fontFamily;
            SelectedFontSize = fontSize;
            GifWidth = gifWidth;
            GifHeight = gifHeight;
            Loop = loop;
        }

        public string Name
        {
            get => _name;
            set => Set(() => Name, ref _name, value);
        }

        public int SelectedFontSize
        {
            get => _selectedFontSize;
            set => Set(() => SelectedFontSize, ref _selectedFontSize, value);
        }

        public int GifHeight
        {
            get => _gifHeight;
            set => Set(() => GifHeight, ref _gifHeight, value);
        }

        public int GifWidth
        {
            get => _gifWidth;
            set => Set(() => GifWidth, ref _gifWidth, value);
        }

        public FontFamily SelectedFont
        {
            get => _selectedFont;
            set => Set(() => SelectedFont, ref _selectedFont, value);
        }

        private bool _loop;
        public bool Loop
        {
            get => _loop;
            set => Set(() => Loop, ref _loop, value);
        }
        public override string ToString()
        {
            return $@"
            \n Name: {_name},
            \n Font: {_selectedFont.Name}
            \n Font Size: {_selectedFont},
            \n Gif Width: {_gifWidth},
            \n Gif Height: {_gifHeight},
            \n Loop: {_loop},
            \n";
        }
    }
}