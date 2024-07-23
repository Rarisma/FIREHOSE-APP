using HYDRANT.Definitions;
using Microsoft.UI;

namespace FirehoseApp.UI;

public sealed partial class ArticleView : Page
{

    private Article Article;
    public ArticleView(Article Data)
    {
        Data.Url = Glob.Model.Proxy + Data.Url;
        InitializeComponent();
        Article = Data;

        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
        {
            Background = new SolidColorBrush(Colors.Black);
        }
        else { Background = new SolidColorBrush(Colors.LightGray); }
    }
    
    private async void WV2_OnLoaded(object sender, RoutedEventArgs e)
    {
        //WORKAROUND: Fix sign-ins for the verge and co on mobile
        //Sign-ins can be blocked based on useragent, so we just set them to 
        //basic desktop ones
        await WV2.EnsureCoreWebView2Async();
        WV2.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" +
                                           " AppleWebKit/537.36 (KHTML, like Gecko) " +
                                           "Chrome/126.0.0.0 Safari/537.36 Edg/126.0.2592.113";
    }
}
