using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using T2G;
using Text2GifGenerator.Tools;
using FontStyle = System.Drawing.FontStyle;

namespace Text2GifGenerator
{
    public class MainWindowViewModel : ObservableObject
    {
        #region Private Fields

        private readonly TextToImageConverter _imageConverter;
        private List<Image> _images = new List<Image>();
        private int _sleepAmount;
        private LibraryEnums.FLowDirection _textFlowDirection;
        private int _currentProgressAmount;
        private string _inputText = "Hello World";

        private ObservableCollection<AcceptableQualityOptions> _qualityOptions =
            new ObservableCollection<AcceptableQualityOptions>();

        private AcceptableQualityOptions _selectedQuality;
        private Image _displayedImage;
        private bool _loop;

        private RelayCommand _showQualityWindowCommand;
        private RelayCommand _generateCommand;
        private RelayCommand _downCommand;
        private RelayCommand _upCommand;
        private RelayCommand _saveCommand;
        private RelayCommand _extractCommand;

        #endregion Private Fields

        public MainWindowViewModel()
        {
            _imageConverter = new TextToImageConverter();
            SleepAmount = (int) Math.Round(100f / 90);
            QualityOptions.Add(new AcceptableQualityOptions("Low", new FontFamily("Tahoma"), 12, 128, 36, true));
            QualityOptions.Add(new AcceptableQualityOptions("Medium", new FontFamily("Tahoma"), 24, 256, 48, true));
            QualityOptions.Add(new AcceptableQualityOptions("High", new FontFamily("Tahoma"), 36, 512, 72, true));

            SelectedQuality = QualityOptions.First();
        }

        #region XAML Properties

        public string InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        public ObservableCollection<FontFamily> AvailableFontsCollection =>
            new ObservableCollection<FontFamily>(new InstalledFontCollection().Families);

        public ObservableCollection<AcceptableQualityOptions> QualityOptions
        {
            get => _qualityOptions;
            set => SetProperty(ref _qualityOptions, value);
        }

        public AcceptableQualityOptions SelectedQuality
        {
            get => _selectedQuality;
            set => SetProperty(ref _selectedQuality, value);
        }


        public LibraryEnums.FLowDirection TextFlowDirection
        {
            get => _textFlowDirection;
            set => SetProperty(ref _textFlowDirection, value);
        }

        public Image DisplayedImage
        {
            get => _displayedImage;
            set => SetProperty(ref _displayedImage, value);
        }

        public bool Loop
        {
            get => _loop;
            set => SetProperty(ref _loop, value);
        }

        public int SleepAmount
        {
            get => _sleepAmount;
            set => SetProperty(ref _sleepAmount, value);
        }

        public int MaxProgressAmount { get; set; } = 100;

        public int CurrentProgressAmount
        {
            get => _currentProgressAmount;
            set => SetProperty(ref _currentProgressAmount, value);
        }

        #endregion XAML Properties

        #region Commands

        public ICommand ShowQualityWindowCommand => _showQualityWindowCommand ??= new RelayCommand(() =>
        {
            var win = new QualitySettingsWindow
                {DataContext = new QualitySettingsViewModel(SelectedQuality)};
            win.Show();
        });

        public ICommand GenerateCommand => _generateCommand ??= new RelayCommand(() =>
        {
            _images.Clear();

            var settings = new TextToImageSettings
            {
                Font = new Font(SelectedQuality.SelectedFont.Name, SelectedQuality.SelectedFontSize, FontStyle.Bold),
                MaxWidth = SelectedQuality.GifWidth,
                MaxHeight = SelectedQuality.GifHeight,
                Loop = true,
                Background = Color.White,
                Foreground = Color.FromArgb(255, 19, 107, 117),
                FlowDirection = TextFlowDirection
            };

            _images = _imageConverter.DrawText(settings, InputText);

            DisplayGif();
        });

        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(async () =>
        {
            var progressIndicator = new Progress<ProgressReport>(ReportProgress);
            var dlg = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".gif",
                InitialDirectory = string.Empty,
            };

            if (dlg.ShowDialog() != true) return;

            var watch = new Stopwatch();
            watch.Start();

            var result = await _imageConverter
                .CreateGif(_images, dlg.FileName, progressIndicator, _selectedQuality.Loop)
                .ConfigureAwait(true);

            watch.Stop();

            Console.WriteLine($@"Settings: {_selectedQuality}");
            Console.WriteLine($@"Created gif. Took {watch.ElapsedMilliseconds} milliseconds.");

            if (!result) return;
            CurrentProgressAmount = 0;
        }, () => _images.Count > 0);


        public ICommand ExtractCommand => _extractCommand ??= new RelayCommand(() =>
        {
            // do nothing
            return;
            // var filePath = @"C:\Users\Edson\Documents\test_20200912_2158\flow_direction\hello_world_low_downtoup.gif";
            // _imageConverter.ExtractGif(filePath);
        });

        #endregion Commands

        #region Private Functions

        private void DisplayGif()
        {
            Application.Current?.Dispatcher.Invoke(async () =>
            {
                foreach (var image in _images)
                {
                    await Task.Run(() =>
                    {
                        DisplayedImage = image;
                        Thread.Sleep(_sleepAmount);
                    });
                }

                if (Loop) DisplayGif();
            });
        }

        private void ReportProgress(ProgressReport progress)
        {
            OnPropertyChanged(nameof(MaxProgressAmount));
            MaxProgressAmount = progress.TotalProgressAmount;
            CurrentProgressAmount = progress.CurrentProgressAmount;
        }

        #endregion Private Functions
    }
}