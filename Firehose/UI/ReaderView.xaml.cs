using Windows.Media.Core;
using Windows.Media.Playback;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Viewmodels;

namespace FirehoseApp.UI;

/// <summary>
/// Simple Reader mode
/// </summary>
public sealed partial class ReaderMode : Page
{
    private ShellVM ShellVM = Ioc.Default.GetRequiredService<ShellVM>();
    public ReaderMode()
    {
        InitializeComponent();
    }
    
    private void LoadAudio(FrameworkElement FrameworkElement, object Args)
    {
        var s = MediaSource.CreateFromUri(new Uri(ShellVM.Hallon.Endpoint +
                                                  "/Audio/GetAudioForArticle?Url=" +
                                                  Uri.EscapeDataString(ShellVM.CurrentArticle.URL)
                                                  + "&ArticleText=True"));
        Audio.Source = s;
        Audio.AreTransportControlsEnabled = true;
    }
}
