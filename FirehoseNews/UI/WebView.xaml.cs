using HYDRANT.Definitions;
using Microsoft.UI;

namespace FirehoseNews.UI;

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
}
