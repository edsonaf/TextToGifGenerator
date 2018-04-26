using System.Windows;

namespace Text2GifGenerator.Tools
{
  /// <summary>
  ///   Interaction logic for QualitySettingsWindow.xaml
  /// </summary>
  public partial class QualitySettingsWindow : Window
  {
    public QualitySettingsWindow()
    {
      InitializeComponent();
    }

    public QualitySettingsViewModel QualitySettingsViewModel
    {
      set => DataContext = value;
    }
  }
}