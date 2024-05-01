using System.Collections.ObjectModel;
using Windows.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Imaging;

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

    }

    private async void LoadDataAsync(string Filter = "ORDER BY PUBLISH_DATE DESC")
    {
        try
        {
            Articles.Clear();
            List<Article> arts = await Article.GetArticles(100, Offset, Filter);
            
            foreach (Article article in arts) { Articles.Add(article); }
        }
        catch (Exception ex)
        {
            Articles.Add(new()
            {
                Title = "Failed to load articles!",
                RSSSummary = ex.Message,
                PublisherID = -1,
                ImageURL = "?",
                PublishDate = new DateTime(1911, 11, 19),
            });
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Opens a tapped article object.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ArticleTapped(object sender, TappedRoutedEventArgs e)
    {
        try
        {
            Article Data = (Article)((FrameworkElement)e.OriginalSource).DataContext;
            OpenArticle(Data);
        }
        catch (Exception ex)
        {
            Glob.Log(Glob.LogLevel.Sev, $"Failed to navigate to tapped article.\n\n" +
                $"Exception {ex.Message}\n\n{ex.Source}\n\n{ex.Data}");
        }
    }

    /// <summary>
    /// Load more stories
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadMorePressed(object sender, RoutedEventArgs e)
    {
        Offset += Articles.Count;
        LoadDataAsync();
        ArticleScroller.ChangeView(0, 0, ArticleScroller.ZoomFactor);
    }

    /// <summary>
    /// Opens an article in the users preferred mode
    /// </summary>
    /// <param name="Data">Article Object to open</param>
    private void OpenArticle(Article Data)
    {
        Glob.Log(Glob.LogLevel.Inf,
            $"Navigating to {Data.Url} (Reader Mode: {Glob.Model.AlwaysOpenReader})");
        if (Glob.Model.AlwaysOpenReader)
        {
            Glob.NaviStack.Push(new ReaderMode(Data));
        }
        else
        {
            Glob.NaviStack.Push(new ArticleView(Data));
        }

        //Navigate to view
        Glob.DoNavi();
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        Glob.NaviStack.Push(new Preferences());
        Glob.DoNavi();
    }

    private void ChangeFilter(object sender, RoutedEventArgs e)
    {
        string Filter;

        //Clear filter buttons
        foreach (var FitlerButton in Filters.Children)
        {
            (FitlerButton as AppBarButton)!.Background = new SolidColorBrush(Colors.Transparent);
            (FitlerButton as AppBarButton)!.Foreground = new SolidColorBrush(Colors.White);
        }
        //Bookmarks button isn't in the filters stack panel so clear them manually.
        BookmarksButton.Background = new SolidColorBrush(Colors.Transparent);
        BookmarksButton.Foreground = new SolidColorBrush(Colors.White);

        //Set filter button background and foreground.
        ((AppBarButton)sender).Background = new SolidColorBrush(Colors.White);
        ((AppBarButton)sender).Foreground = new SolidColorBrush(Colors.Black);

        //Set correct filter
        switch (((AppBarButton)sender).Content)
        {
            case "Latest":
                Filter = "ORDER BY PUBLISH_DATE DESC";
                break;
            case "Headlines":
                Filter = "WHERE HEADLINE = 1 ORDER BY PUBLISH_DATE DESC";
                break;
            case "Hot":
                Filter = "WHERE HEADLINE = 1 ORDER BY PUBLISH_DATE DESC";
                break;
            case "Today":
                Filter = "WHERE PUBLISH_DATE > NOW() - INTERVAL 1 DAY";
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

        LoadDataAsync(Filter);
    }

    /// <summary>
    /// Opens a quick view of an article showing the title,
    /// image and the AI Summary of the article.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OpenQuickView(object sender, RightTappedRoutedEventArgs e)
    {

        Article article = (e.OriginalSource as FrameworkElement)?.DataContext as Article;

        StackPanel UI = new();

        //Show image if we have access to one.
        if (article.ImageURL != "?")
        {
            UI.Children.Add(new StackPanel
            {
                Children =
                {
                    new Image
                    {
                        Source = new BitmapImage(new Uri(article.ImageURL))
                    },
                },
                CornerRadius = new(8)
            });
        }

        
        //Create summary box
        UI.Children.Add(new Border
        {
                Margin = new(0,20,0,20),
                CornerRadius = new(12),
                Background = new SolidColorBrush(Color.FromArgb(128,22,22,22)),
                Child = new TextBlock
                {
                    Text = ((e.OriginalSource as FrameworkElement).DataContext as Article).Summary,
                    Margin = new(10),
                    TextWrapping = TextWrapping.Wrap
                }
        });

        //Create content dialog
        ContentDialog CD = new()
        {
            Content = new ScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = UI
            },
            XamlRoot = this.XamlRoot,
            Title = $"Summary for {article.Title}",
            PrimaryButtonText = "Open Article",
            SecondaryButtonText = "Close"
        };

        //Show dialog.
        if (await CD.ShowAsync() == ContentDialogResult.Primary)
        {
            OpenArticle(article);
        }
    }
}
