using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Viewmodels;
using HYDRANT.Definitions;

namespace FirehoseApp.UI.Dialogs;

public sealed partial class QuickView : Page
{
    private ThemeVM Themer = Ioc.Default.GetRequiredService<ThemeVM>();

    /// <summary>
    /// Article we are showing view for.
    /// </summary>
    public Article Article;
    public QuickView(Article Article)
    {
        this.Article = Article;

        InitializeComponent();
        
        //Hide image box if needed.
        if (Article.ImageURL == "?") { ImageBox.Visibility = Visibility.Collapsed; }
    }
}
