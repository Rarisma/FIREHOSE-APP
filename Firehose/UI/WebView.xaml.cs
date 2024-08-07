using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using HYDRANT.Definitions;
using Microsoft.UI;

namespace FirehoseApp.UI;

public sealed partial class ArticleView : Page
{
    PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

    private Article Article;
    public ArticleView(Article Data)
    {
        Data.Url = Pref.Proxy + Data.Url;
        InitializeComponent();
        Article = Data;

        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
        {
            Background = new SolidColorBrush(Colors.Black);
        }
        else { Background = new SolidColorBrush(Colors.LightGray); }
    }
}
