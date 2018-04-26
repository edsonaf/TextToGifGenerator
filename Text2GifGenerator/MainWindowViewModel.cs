using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Text2GifGenerator.Tools;
using TextToGifGenerator;

namespace Text2GifGenerator
{
  public class MainWindowViewModel : ViewModelBase
  {
    #region Private Fields

    private readonly TextToImageConverter _imageConverter;
    private List<Image> _images = new List<Image>();
    private int _sleepAmount = 10;
    private string _inputText = "";
    private ObservableCollection<AcceptableQualityOptions> _qualityOptions = new ObservableCollection<AcceptableQualityOptions>();
    private AcceptableQualityOptions _selectedQuality;
    private Image _displayedImage;
    private bool _isBusy;
    private bool _loop = true;

    private RelayCommand _showQualityWindowCommand;
    private RelayCommand _generateCommand;
    private RelayCommand _upCommand;
    private RelayCommand _downCommand;

    #endregion Private Fields
    
    public MainWindowViewModel()
    {
      _imageConverter = new TextToImageConverter();

      QualityOptions.Add(new AcceptableQualityOptions("Low", new FontFamily("Tahoma"), 12, 128, 36));
      QualityOptions.Add(new AcceptableQualityOptions("Medium", new FontFamily("Tahoma"), 24, 256, 48));
      QualityOptions.Add(new AcceptableQualityOptions("High", new FontFamily("Tahoma"), 36, 512, 72));

      SelectedQuality = QualityOptions.First();
    }

    #region XAML Properties

    public string InputText
    {
      get => _inputText;
      set => Set(() => InputText, ref _inputText, value);
    }

    public ObservableCollection<FontFamily> AvailableFontsCollection => new ObservableCollection<FontFamily>(new InstalledFontCollection().Families);

    public ObservableCollection<AcceptableQualityOptions> QualityOptions
    {
      get => _qualityOptions;
      set => Set(() => QualityOptions, ref _qualityOptions, value);
    }

    public AcceptableQualityOptions SelectedQuality
    {
      get => _selectedQuality;
      set => Set(() => SelectedQuality, ref _selectedQuality, value);
    }

    public Image DisplayedImage
    {
      get => _displayedImage;
      set => Set(() => DisplayedImage, ref _displayedImage, value);
    }

    public bool IsBusy
    {
      get => _isBusy;
      set => Set(() => IsBusy, ref _isBusy, value);
    }

    public int SleepAmount
    {
      get => _sleepAmount;
      set => Set(() => SleepAmount, ref _sleepAmount, value);
    }

    public bool Loop
    {
      get => _loop;
      set => Set(() => Loop, ref _loop, value);
    }

    #endregion XAML Properties

    #region Commands

    public ICommand ShowQualityWindowCommand => _showQualityWindowCommand ?? (_showQualityWindowCommand = new RelayCommand(() =>
    {
      QualitySettingsWindow win = new QualitySettingsWindow {DataContext = new QualitySettingsViewModel(SelectedQuality)};
      win.Show();
    }));

    public ICommand GenerateCommand => _generateCommand ?? (_generateCommand = new RelayCommand(async () =>
    {
      _images.Clear();

      TextToImageSettings settings = new TextToImageSettings()
      {
        Font = new Font(SelectedQuality.SelectedFont.Name, SelectedQuality.SelectedFontSize, FontStyle.Bold),
        MaxWidth = SelectedQuality.GifWidth,
        MaxHeight = SelectedQuality.GifHeight,
        Loop = true,
        Background = Color.White,
        Foreground = Color.Red
      };
      
      _images = _imageConverter.DrawText(settings, InputText);

      while (Loop)
      {
        try
        {
          await DisplayGif();
        }
        catch
        {
          IsBusy = false;
          Loop = false;
        }
      }
    }));

    public ICommand UpCommand => _upCommand ?? (_upCommand = new RelayCommand(() =>
    {
      if (SleepAmount + 5 >= 100)
      {
        SleepAmount = 100;
      }
      else
      {
        SleepAmount += 5;
      }
    }));

    public ICommand DownCommand => _downCommand ?? (_downCommand = new RelayCommand(() =>
    {
      if (SleepAmount - 5 <= 1)
      {
        SleepAmount = 1;
      }
      else
      {
        SleepAmount -= 5;
      }
    }));

    #endregion Commands
    
    #region Private Functions

    private async Task DisplayGif(int indexOfDisplayedImage = 0)
    {
      IsBusy = true;

      await Task.Run(async () =>
      {
        foreach (Image image in _images)
        {
          await Task.Run(() =>
          {
            DisplayedImage = image;
            Thread.Sleep(_sleepAmount);
          });
        }

        Thread.Sleep(250);

        DisplayedImage = null;
        DisplayedImage = _images[indexOfDisplayedImage];
        IsBusy = false;
      });
    }

    #endregion Private Functions
    
  }

  public class AcceptableQualityOptions : ObservableObject
  {
    private int _gifHeight;

    private int _gifWidth;

    private string _name = "Undefined";

    private FontFamily _selectedFont = new FontFamily("Tahoma");

    private int _selectedFontSize;

    public AcceptableQualityOptions(string name, FontFamily fontFamily, int fontSize, int gifWidth, int gifHeight)
    {
      Name = name;
      SelectedFont = fontFamily;
      SelectedFontSize = fontSize;
      GifWidth = gifWidth;
      GifHeight = gifHeight;
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
  }
}