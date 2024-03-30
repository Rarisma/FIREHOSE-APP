using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Input;
using Windows.UI.Core;

namespace VESTIGENEWS;

public sealed partial class ArticleList : Page
{
    public ObservableCollection<Article> Articles { get; set; }
    public ArticleList()
    {
        Articles = new();
        LoadDataAsync();
        InitializeComponent();

#if __ANDROID__
        var manager = SystemNavigationManager.GetForCurrentView();
        manager.BackRequested += OnBackRequested;
#endif
    }

    private void OnBackRequested(object? sender, BackRequestedEventArgs e)
    {
        if (Glob.NaviStack.Count > 1) { Glob.FuckGoBack(); }
    }

    private async void LoadDataAsync()
    {
        try
        {
            Articles.Clear();
            var arts = await Article.GetArticles(100);
            foreach (Article article in arts.OrderBy(item => new Random().Next()))
            {
                if (article.ImageURL == null)
                {
                    article.ImageURL = "Assets/Placeholder.png";
                }
                Articles.Add(article);

            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.ToString());
        }
    }

    private void InvokeArticle(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {

    }

    private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        Article Data = (e.OriginalSource as FrameworkElement).DataContext as Article;
        ArticleView V = new ArticleView(Data);
        Glob.NaviStack.Push(V);
        Glob.DoNavi();
    }
}
