using CommunityToolkit.Mvvm.DependencyInjection;
using Firehose.UI.Controls;
using Firehose.Viewmodels;
using HYDRANT.Definitions;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Uno.Extensions;

namespace Firehose.UI;

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
        
        if (await Glob.OpenContentDialog(CD) == ContentDialogResult.Primary)
        {
            ShellVM.PublisherID = ((CD.Content as PublisherFilter).Content as ListView)!.SelectedIndex + 1;
            ShellVM.FilterExtension = $"PUBLISHER_ID = {ShellVM.PublisherID}";

        }
        else
        {
            ShellVM.PublisherID = -1;
            SourcesButton.Content = "Filter Publisher";
            ShellVM.FilterExtension = "";
        }
        ShellVM.FilterOrder = ShellVM.Filters[0].FilterOrder;
        ShellVM.Offset = 0;
        UpdateButtons(ShellVM.Filters[0]);
        ShellVM.LoadArticleDataCommand.Execute(null);
    }

    public ArticleList()
    {
        ShellVM.UpdateButtonsDelegate = new(UpdateButtons);
        ShellVM.Articles = new();
        InitializeComponent();
        ChangeFilter(ShellVM.Filters[0], new());
    }


    /// <summary>
    /// Opens a tapped article object.
    /// </summary>
    private void ArticleTapped(object sender, TappedRoutedEventArgs e)
    {
        //Get article object
        Article Data = (Article)((FrameworkElement)e.OriginalSource).DataContext;
        ShellVM.OpenArticle(Data); //Open article.
    }


    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        Glob.NaviStack.Push(new Preferences());
        Glob.DoNavi();
    }
    
    /// <summary>
    /// Updates UI on top row of buttons.
    /// </summary>
    /// <param name="Button">Button to set as selected.</param>
    public void UpdateButtons(Button Button)
    {
        //Hide no bookmarks message.
        NoBookmarks.Text = " ";

        //Show load more button
        LoadMoreButton.Visibility = Visibility.Visible;

        //Clear filter buttons
        foreach (var FilterButton in ShellVM.Filters)
        {
            FilterButton.Background = new SolidColorBrush(Colors.Transparent);
            FilterButton.Foreground = Themer.SecondaryBrush;
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
        UpdateButtons((Button)sender);

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
        UpdateButtons(sender as Button);
        ShellVM.Articles.Clear();
        ShellVM.Offset = 0;
        LoadMoreButton.Visibility = Visibility.Collapsed;
        ShellVM.Articles.AddRange(Glob.Model.BookmarkedArticles);

        //Show no bookmarks text, so it's not a blank screen
        if (Glob.Model.BookmarkedArticles.Count == 0)
        {
            NoBookmarks.Text = "You have no bookmarked articles.";
        }
    }
    
    private async void FilterSource(object sender, RoutedEventArgs e) => await SetPublisherFilter();
}
