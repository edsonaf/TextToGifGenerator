using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using TextToGifGenerator;


namespace Text2GifGenerator
{
  public class MainWindowViewModel : ViewModelBase
  {
    private List<Image> _images = new List<Image>();
    private readonly ITextToImageConverter _imageConverter;

    public MainWindowViewModel()
    {
      _imageConverter = new TextToImageConverter();
    }

    #region XAML Properties

    private string _text = "";
    public string Text
    {
      get => _text;
      set => Set(() => Text, ref _text, value);
    }

    public ObservableCollection<FontFamily> AvailableFontsCollection => new ObservableCollection<FontFamily>(new InstalledFontCollection().Families);

    public FontFamily SelectedFont
    {
      get => _selectedFont;
      set => Set(() => SelectedFont, ref _selectedFont, value);
    }

    public int SelectedFontSize
    {
      get => _selectedFontSize;
      set => Set(() => SelectedFontSize, ref _selectedFontSize, value); 
    }

    private Image _displayedImage;
    public Image DisplayedImage
    {
      get => _displayedImage;
      set => Set(() => DisplayedImage, ref _displayedImage, value);
    }

    private bool _canExecuteGenerateCommand = true;
    public bool CanExecuteGenerateCommand
    {
      get => _canExecuteGenerateCommand;
      set => Set(() => CanExecuteGenerateCommand, ref _canExecuteGenerateCommand, value);
    }

    #endregion XAML Properties

    private RelayCommand<object> _generateCommand;
    private FontFamily _selectedFont;
    private int _selectedFontSize;

    public ICommand GenerateCommand => _generateCommand ?? (_generateCommand = new RelayCommand<object>(async param =>
    {
      _images.Clear();
      var mainWindowWidth = ((MainWindow)param).Width;
      if ((int)mainWindowWidth == 0) mainWindowWidth = 128;

      var imageControlHeight = ((MainWindow)param).MainGrid.RowDefinitions[2].ActualHeight;
      if ((int)imageControlHeight <= 0) imageControlHeight = 36;

      _images = _imageConverter.DrawText(Text, new Font(SelectedFont.Name, SelectedFontSize, FontStyle.Bold), (int)mainWindowWidth, (int)imageControlHeight);

      await DisplayGif((int)mainWindowWidth - 1);
    }));

    private async Task DisplayGif(int indexOfDisplayedImage)
    {
      var i = 10;
      CanExecuteGenerateCommand = false;

      await Task.Run(async () =>
      {
        foreach (var image in _images)
        {
          await Task.Run(() =>
          {
            DisplayedImage = image;
            Thread.Sleep(i);
          });
        }

        Thread.Sleep(1000);

        DisplayedImage = null;
        DisplayedImage = _images[indexOfDisplayedImage];
        CanExecuteGenerateCommand = true;
      });
    }

  }
}
