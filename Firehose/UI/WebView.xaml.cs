using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.Viewmodels;
using Microsoft.UI;

namespace FirehoseApp.UI;

public sealed partial class ArticleWebView : Page
{
    PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();
    ShellVM SVM = Ioc.Default.GetRequiredService<ShellVM>();
    public ArticleWebView()
    {
        SVM.CurrentArticle.URL = Pref.Proxy + SVM.CurrentArticle.URL;
        InitializeComponent();

        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
        {
            Background = new SolidColorBrush(Colors.Black);
        }
        else { Background = new SolidColorBrush(Colors.LightGray); }
    }
}
