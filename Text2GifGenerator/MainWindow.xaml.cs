namespace Text2GifGenerator
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    public MainWindowViewModel MainWindowViewModel
    {
      set => DataContext = value;
    }
  }
}