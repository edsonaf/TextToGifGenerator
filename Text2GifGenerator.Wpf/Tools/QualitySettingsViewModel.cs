namespace Text2GifGenerator.Tools
{
  public class QualitySettingsViewModel
  {
    public QualitySettingsViewModel(AcceptableQualityOptions qualityOptions)
    {
      SelectedQuality = qualityOptions;
    }

    public AcceptableQualityOptions SelectedQuality { get; set; }
  }
}