using System.Collections.ObjectModel;
using System.Globalization;
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

    private async void LoadDataAsync(bool Shuffle = true, string Filter = "ORDER BY PUBLISH_DATE DESC")
    {
        if (Glob.Model.AlwaysShuffle) { Shuffle = true; }

        try
        {
            Articles.Clear();
            List<Article> arts = await Article.GetArticles(100, Offset);
            
            //Shuffle Articles if enabled
            if (Shuffle) { arts = arts.OrderBy(item => new Random().Next()).ToList();}


            foreach (Article article in arts) { Articles.Add(article); }
        }
        catch (Exception ex)
        {
            Articles.Add(new()
            {
                Title = "Failed to load articles!",
                RssSummary = ex.Message,
                Publisher = "N/A",
                ImageURL = "?",
                PublishDate = new DateTime(1911, 11, 19),
            });
            Console.WriteLine(ex.ToString());
        }
    }
    private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        try
        {
            Article Data = (Article)((FrameworkElement)e.OriginalSource).DataContext;
            if (Glob.Model.AlwaysOpenReader)
            {
                Glob.NaviStack.Push(new ReaderMode(Data));
            }
            else
            {
                Glob.NaviStack.Push(new ArticleView(Data));
            }
            Glob.DoNavi();
        }
        catch (Exception ex)
        {
            Glob.Log(Glob.LogLevel.Sev, $"Failed to navigate to tapped article.\n\n" +
                $"Exception {ex.Message}\n\n{ex.Source}\n\n{ex.Data}");
        }
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
        //ScrollViewer scrollViewer = (ScrollViewer)sender;
        //if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
        //{
        //    Offset += Articles.Count;
        //    LoadDataAsync();
        //}
    }

    private void LoadMorePressed(object sender, RoutedEventArgs e)
    {
        Offset += Articles.Count;
        LoadDataAsync();
        ArticleScroller.ChangeView(0, 0, ArticleScroller.ZoomFactor);
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        Glob.NaviStack.Push(new Preferences());
        Glob.DoNavi();
    }

    private void ChangeFilter(object sender, RoutedEventArgs e)
    {
        string Filter;
        bool Shuffle = true;

        switch (((AppBarButton)sender).Content)
        {
            case "Latest":
                Filter = "ORDER BY PUBLISH_DATE DESC";
                Shuffle = false;
                break;
            case "Headlines":
                Filter = "WHERE HEADLINE = 1 ORDER BY PUBLISH_DATE DESC";
                Shuffle = false;
                break;
            case "Hot":
                Filter = "WHERE HEADLINE = 1 ORDER BY PUBLISH_DATE DESC";
                Shuffle = false;
                break;
            case "Today":
                Filter = "WHERE PUBLISH_DATE > NOW() - INTERVAL 1 DAY";
                Shuffle = true;
                break;
            case "Bookmarked":
                Articles.Clear();
                foreach (Article Art in Glob.Model.bookmarkedArticles)
                {
                    Articles.Add(Art);
                }
                return;
            //Standard shuffle if something goes wrong.
            default:
                LoadDataAsync();
                return;
        }

        LoadDataAsync(Shuffle, Filter);
    }
}
