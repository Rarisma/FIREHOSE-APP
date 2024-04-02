using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Input;
using Windows.UI.Core;

namespace VESTIGENEWS;

public sealed partial class ArticleList : Page
{
    private int Offset = 0;
    public ObservableCollection<Article> Articles { get; set; }
    public ArticleList()
    {
        Articles = new();
        LoadDataAsync();
        InitializeComponent();

#if __ANDROID__
        SystemNavigationManager.GetForCurrentView().BackRequested += Glob.OnBackRequested;
#endif
    }

    private async void LoadDataAsync()
    {
        try
        {
            Articles.Clear();
            var arts = await Article.GetArticles(100, Offset);
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
    private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        Article Data = (e.OriginalSource as FrameworkElement).DataContext as Article;
        Glob.NaviStack.Push(new ArticleView(Data));
        Glob.DoNavi();
    }

    /// <summary>
    /// Check if we have hit the bottom.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ScrollToEnd(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        //TODO: Reimplement to add infinite scroll.
        return;
        ScrollViewer scrollViewer = (ScrollViewer)sender;
        if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
        {
            Offset += Articles.Count;
            LoadDataAsync();
        }
    }

    private void LoadMorePressed(object sender, RoutedEventArgs e)
    {
        Offset += Articles.Count;
        LoadDataAsync();
        ArticleScroller.ChangeView(0, 0, ArticleScroller.ZoomFactor);
    }
}
