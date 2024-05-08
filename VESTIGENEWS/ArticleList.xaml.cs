using System.Collections.ObjectModel;
using Windows.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using Uno.Extensions;
using VESTIGENEWS.Controls;

namespace VESTIGENEWS;

public sealed partial class ArticleList : Page
{
    #region Theming
    /// <summary>
    /// Current app theme
    /// </summary>
    public StatusBarForegroundTheme Theme
    {
        get
        {
            //Convert theme to status foreground theme
            return Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? StatusBarForegroundTheme.Light
                : StatusBarForegroundTheme.Dark;
        }
    }

    /// <summary>
    /// Status bar color (android/ios)
    /// </summary>
    public SolidColorBrush StatusBarBackground
    {
        get
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark ? new(Colors.Black) : new(Colors.White);
        }
    }
    #endregion

    #region PublisherFilter
    /// <summary>
    /// Are filtering by publisher?
    /// </summary>
    public bool PublisherFilter = false;

    /// <summary>
    /// ID of publisher filter to filter for
    /// </summary>
    public int PublisherID = -1;

    /// <summary>
    /// Extra filter for use while filtering publications
    /// </summary>
    public string ExFilter = "";

    /// <summary>
    /// Sets ex filter to filter via publications
    /// </summary>
    /// <returns></returns>
    public async Task SetPublisherFilter()
    {
        ContentDialog CD = new()
        {
            XamlRoot = this.XamlRoot,
            Title = "Filter by publisher",
            Content = new Controls.PublisherFilter(),
            PrimaryButtonText = "Filter by Publisher",
            SecondaryButtonText = "Clear publisher filter",
        };

        if (await CD.ShowAsync() == ContentDialogResult.Primary)
        {
            PublisherFilter = true;
            PublisherID = ((CD.Content as PublisherFilter).Content as ListView)!.SelectedIndex + 1;
        }
        else
        {
            PublisherFilter = false;
            PublisherID = -1;
        }
    }
    #endregion

    private int Offset = 0;
    public ObservableCollection<Article> Articles { get; set; }
    public ArticleList()
    {
        Articles = new();
        InitializeComponent();
        ChangeFilter(LatestButton, new());
    }

    /// <summary>
    /// Loads more data from the server.
    /// </summary>
    /// <param name="Filter">Filter to sort by.</param>
    private async void LoadDataAsync(string Filter = "ORDER BY PUBLISH_DATE DESC")
    {
        try
        {
            Articles.Clear(); //Reset collection

            //Load articles and filter articles from the future.
            Articles.AddRange((await 
                Article.GetArticlesFromHallon(Glob.Model.ArticleFetchLimit, Offset, Filter))
                .Where(x => DateTime.Now.AddHours(3) > x.PublishDate));
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
    private async void ArticleTapped(object sender, TappedRoutedEventArgs e)
    {
        try
        {
            //Get article object
            Article Data = (Article)((FrameworkElement)e.OriginalSource).DataContext;

            if (Glob.Model.ArticleTapOpensSummary) //Open summary if article is set right
            {
                await QuickViewMenu(Data);
            }
            else //Open article in webview2
            {
                OpenArticle(Data);
            }

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
        //Update offset so we don't load the same articles, call article update
        Offset += Articles.Count; 
        LoadDataAsync(); 

        //Scroll back to top
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

    private async void ChangeFilter(object sender, RoutedEventArgs e)
    {
        LoadMoreButton.Visibility = Visibility.Visible;
        SolidColorBrush ForegroundColor = Application.Current.RequestedTheme == ApplicationTheme.Dark
            ? new SolidColorBrush(Colors.White)
            : new SolidColorBrush(Colors.Black);

        SolidColorBrush BackgroundColor = Application.Current.RequestedTheme == ApplicationTheme.Dark
            ? new SolidColorBrush(Colors.Black)
            : new SolidColorBrush(Colors.White);

        string Filter;

        //Clear filter buttons
        foreach (var FilterButton in Filters.Children)
        {
            (FilterButton as AppBarButton)!.Background = new SolidColorBrush(Colors.Transparent);
            (FilterButton as AppBarButton)!.Foreground = ForegroundColor;
        }
        //Bookmarks button isn't in the filters stack panel so clear them manually.
        BookmarksButton.Background = new SolidColorBrush(Colors.Transparent);
        BookmarksButton.Foreground = ForegroundColor;

        //Set filter button background and foreground.
        ((AppBarButton)sender).Background = ForegroundColor;
        ((AppBarButton)sender).Foreground = BackgroundColor;

        //Set correct filter
        switch (((AppBarButton)sender).Content)
        {
            case "Latest":
                Filter = $"{ExFilter.Replace(" AND", "WHERE")}ORDER BY PUBLISH_DATE DESC";
                break;
            case "Headlines":
                Filter = $"WHERE HEADLINE = 1 {ExFilter}ORDER BY PUBLISH_DATE DESC";
                break;
            case "Hot":
                Filter = $"WHERE HEADLINE = 1  {ExFilter}ORDER BY PUBLISH_DATE DESC";
                break;
            case "Today":
                Filter = $"WHERE PUBLISH_DATE > NOW() - INTERVAL 1 DAY {ExFilter}";
                break;
            case "Business":
                Filter = $"WHERE BUSINESS_RELATED = 1 {ExFilter}ORDER BY PUBLISH_DATE DESC";
                break;
            case "Bookmarked":
                Articles.Clear();
                LoadMoreButton.Visibility = Visibility.Collapsed;
                Articles.AddRange(Glob.Model.BookmarkedArticles);
                return;

            //Standard shuffle if something goes wrong.
            default:
                await SetPublisherFilter();
                if (PublisherFilter)
                {
                    Filter = $"WHERE PUBLISHER_ID = {PublisherID} ORDER BY PUBLISH_DATE DESC";
                    ExFilter = $" AND PUBLISHER_ID = {PublisherID} ";
                }
                else
                {
                    ExFilter = "";
                    ChangeFilter(LatestButton, new());
                    return;
                }
                break;
        }

        if (PublisherFilter)
        {
            SourcesButton.Background = ForegroundColor;
            SourcesButton.Foreground = BackgroundColor;
            SourcesButton.Content = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new Image
                    {
                        Height = 16, Width = 16,
                        Margin = new(0, 0, 10, 0),
                        Source= new BitmapImage(new Uri(Glob.Publications[PublisherID].Favicon))
                    },
                    new TextBlock { Text = Glob.Publications[PublisherID].Name, FontSize = 10}
                }
            };
        }
        else
        {
            SourcesButton.Background = BackgroundColor;
            SourcesButton.Foreground = ForegroundColor;
            SourcesButton.Content = "Filter Publisher";
        }

        LoadDataAsync(Filter);
    }

    #region QuickView

    /// <summary>
    /// Opens a quick view of an article showing the title,
    /// image and the AI Summary of the article.
    /// </summary>
    private async Task QuickViewMenu(Article Article)
    {
        StackPanel UI = new();
        SolidColorBrush Background = Application.Current.RequestedTheme == ApplicationTheme.Dark
            ? new SolidColorBrush(Color.FromArgb(128, 22, 22, 22))
            : new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        // Show image if we have access to one.
        if (Article.ImageURL != "?")
        {
            UI.Children.Add(new StackPanel
            {
                Children =
                {
                    new Image
                    {
                        Source = new BitmapImage(new Uri(Article.ImageURL))
                    },
                },
                CornerRadius = new(8)
            });
        }

        // Create summary box
        UI.Children.Add(new Border
        {
            Margin = new(0, 20, 0, 20),
            CornerRadius = new(12),
            Background = Background,
            Child = new TextBlock
            {
                Text = Article.Summary,
                Margin = new(10),
                TextWrapping = TextWrapping.Wrap
            }
        });

        // Create content dialog
        ContentDialog CD = new()
        {
            Content = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = UI
            },
            XamlRoot = this.XamlRoot,
            Title = $"Summary for {Article.Title}",
            PrimaryButtonText = "Open Article",
            SecondaryButtonText = "Close"
        };

        // Save scroll position
        double previousScrollPosition = ArticleScroller.VerticalOffset;

        // Show dialog
        var result = await CD.ShowAsync();

        // Restore scroll position
        ArticleScroller.ChangeView(null, previousScrollPosition, null);

        // Perform actions based on dialog result
        if (result == ContentDialogResult.Primary)
        {
            OpenArticle(Article);
        }
    }


    /// <summary>
    /// Opens article quick view menu when held
    /// </summary>
    private async void OpenQuickViewHold(object sender, RightTappedRoutedEventArgs e)
    {
        //Get article object
        Article article = ((e.OriginalSource as FrameworkElement)?.DataContext as Article)!;
        
        //Open article
        await QuickViewMenu(article);
    }

    /// <summary>
    /// Opens article quick view menu when swiped.
    /// </summary>
    private async void OpenQuickViewSwipe(object sender, SwipeItemInvokedEventArgs args)
    {
        //Open menu with appropriate article found via tag.
        await QuickViewMenu(Articles.First(x => x.Url == args.SwipeControl.Tag.ToString()));
    }
    #endregion


    private void ShowSources(object sender, RoutedEventArgs e)
    {
    }
}
