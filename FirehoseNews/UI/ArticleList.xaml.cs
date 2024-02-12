using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseNews.UI.Controls;
using FirehoseNews.Viewmodels;
using HYDRANT.Definitions;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Uno.Extensions;

namespace FirehoseNews.UI;

public sealed partial class ArticleList : Page
{
    private ThemeVM Themer = Ioc.Default.GetRequiredService<ThemeVM>();
    private ShellVM ShellVM = Ioc.Default.GetRequiredService<ShellVM>();
    
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
            Content = new PublisherFilter(),
            PrimaryButtonText = "Filter by Publisher",
            SecondaryButtonText = "Clear publisher filter",
        };
        
        if (await CD.ShowAsync() == ContentDialogResult.Primary)
        {
            ShellVM.PublisherID = ((CD.Content as PublisherFilter).Content as ListView)!.SelectedIndex + 1;
        }
        else
        {
            ShellVM.PublisherID = -1;
            SourcesButton.Content = "Filter Publisher";
        }
    }

    public ArticleList()
    {
        ShellVM.Articles = new();
        InitializeComponent();
        ChangeFilter(LatestButton, new());
    }


    /// <summary>
    /// Opens a tapped article object.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ArticleTapped(object sender, TappedRoutedEventArgs e)
    {
        //Get article object
        Article Data = (Article)((FrameworkElement)e.OriginalSource).DataContext;
        ShellVM.OpenArticle(Data); //Open article.
    }

    /// <summary>
    /// Load more stories
    /// </summary>
    private void LoadMorePressed(object sender, RoutedEventArgs e)
    {
        //Update offset so we don't load the same articles, call article update
        ShellVM.LoadArticleDataCommand.Execute(null);

        //Scroll back to top
        ArticleScroller.ChangeView(0, 0, ArticleScroller.ZoomFactor);
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        Glob.NaviStack.Push(new Preferences());
        Glob.DoNavi();
    }
    
    /// <summary>
    /// I </summary>
    /// <param name="Button"></param>
    public void UpdateButtons(AppBarButton Button)
    {
        //Clear filter buttons
        foreach (var FilterButton in Filters.Children)
        {
            Button.Background = new SolidColorBrush(Colors.Transparent);
            Button.Foreground = Themer.SecondaryBrush;
        }
        //Bookmarks button isn't in the filters stack panel so clear them manually.
        BookmarksButton.Background = new SolidColorBrush(Colors.Transparent);
        BookmarksButton.Foreground = Themer.SecondaryBrush;
        
        //Set filter by button.
        if (ShellVM.PublisherID != -1)
        {
            SourcesButton.Background = Themer.SecondaryBrush;
            SourcesButton.Foreground = Themer.MainBrush;
            SourcesButton.Content = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new Image
                    {
                        Height = 16, Width = 16,
                        Margin = new(0, 0, 10, 0),
                        Source = new BitmapImage(new Uri(Glob.Publications[ShellVM.PublisherID].Favicon))
                    },
                    new TextBlock { Text = Glob.Publications[ShellVM.PublisherID].Name, FontSize = 10}
                }
            };
        }
        else
        {
            SourcesButton.Background = Themer.MainBrush;
            SourcesButton.Foreground = Themer.SecondaryBrush;
        }
        //Set filter button background and foreground.
        Button.Background = Themer.SecondaryBrush;
        Button.Foreground = Themer.MainBrush;
    }

    private void ChangeFilter(object sender, RoutedEventArgs e)
    {
        ShellVM.Offset = 0;
        LoadMoreButton.Visibility = Visibility.Visible;
        UpdateButtons((AppBarButton)sender);

        //Reset filters to default
        ShellVM.FilterBy = "";
        ShellVM.FilterOrder = "ORDER BY PUBLISH_DATE DESC";

        //Set correct filter
        ShellVM.LoadArticleDataCommand.Execute(null);
    }

    #region QuickView

    /// <summary>
    /// Opens a quick view of an article showing the title,
    /// image and the AI Summary of the article.
    /// </summary>
    private async Task QuickViewMenu(Article Article)
    {
        // Save scroll position
        double previousScrollPosition = ArticleScroller.VerticalOffset;
        
        await ShellVM.OpenQuickView(Article);

        // Restore scroll position
        ArticleScroller.ChangeView(null, previousScrollPosition, null);
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
        await QuickViewMenu(ShellVM.Articles.First(x => x.Url == args.SwipeControl.Tag.ToString()));
    }
    #endregion
    
    private void ArticleList_OnLoaded(object sender, RoutedEventArgs e) => Glob.XamlRoot = this.XamlRoot;
    
    private void ShowBookmarks(object sender, RoutedEventArgs e)
    {
        ShellVM.Articles.Clear();
        ShellVM.Offset = 0;
        LoadMoreButton.Visibility = Visibility.Collapsed;
        ShellVM.Articles.AddRange(Glob.Model.BookmarkedArticles);
    }
    
    private async void FilterSource(object sender, RoutedEventArgs e)
    {
        await SetPublisherFilter();
        if (ShellVM.PublisherID == -1)
        {
            ShellVM.FilterOrder = $" AND PUBLISHER_ID = {ShellVM.PublisherID} ";
        }
        else
        {
            ShellVM.FilterOrder = "";
            ChangeFilter(LatestButton, new());
        }
    }
}
