using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.UI.Controls;
using FirehoseApp.Viewmodels;
using HYDRANT.Definitions;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using Uno.Extensions;

namespace FirehoseApp.UI;

public sealed partial class ArticleList : Page
{
    private ThemeVM Themer = Ioc.Default.GetRequiredService<ThemeVM>();
    private ShellVM ShellVM = Ioc.Default.GetRequiredService<ShellVM>();
    private PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();
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
            ShellVM.PublisherID = ((CD.Content as PublisherFilter)
                .Content as ListView)!.SelectedIndex + 1;
        }
        else
        {
            //Set back to Filter button
            ShellVM.PublisherID = -1;
            SourcesButton.Content = new FontIcon
            {
                Glyph = "\xE71C"
            };
        }
        ShellVM.Offset = 0;
        UpdateButtons(ShellVM.Filters[0]);
        ShellVM.LoadArticleDataCommand.Execute(null);
    }

    public ArticleList()
    {
        ShellVM.LoadAllDataCommand.Execute(null);
        ShellVM.UpdateButtonsDelegate = UpdateButtons;
        ShellVM.Articles = new();
        ShellVM.Offset = 0;
        ShellVM.LoadMoreVisibility = Visibility.Visible;

        //Set correct filter
        InitializeComponent();
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        Glob.DoNavi(new Preferences());
    }
    
    /// <summary>
    /// Updates UI on top row of buttons.
    /// </summary>
    /// <param name="Button">Button to set as selected.</param>
    public void UpdateButtons(Button Button)
    {
        //Hide no bookmarks message.
        ShellVM.LoadMoreVisibility = Visibility.Collapsed;

        //Show load more button
        ShellVM.LoadMoreVisibility = Visibility.Visible;

        //Clear filter buttons
        foreach (var FilterButton in ShellVM.Filters)
        {
            FilterButton.Background = new SolidColorBrush(Colors.Transparent);
            FilterButton.Foreground = Themer.SecondaryBrush;
        }
        //Bookmarks button isn't in the filters stack panel so clear them manually.
        BookmarkButton.Background = Themer.MainBrush;
        BookmarkButton.Foreground = Themer.SecondaryBrush;

        //Set filter by button.
        if (ShellVM.PublisherID != -1)
        {
            SourcesButton.Background = Themer.SecondaryBrush;
            SourcesButton.Foreground = Themer.MainBrush;
            SourcesButton.Content = new Image
            {
                Height = 16,
                Width = 16,
                Source = new BitmapImage(new Uri(Glob.Publications[ShellVM.PublisherID].Favicon))
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

    private void ArticleList_OnLoaded(object sender, RoutedEventArgs e) => Glob.XamlRoot = XamlRoot;
    
    /// <summary>
    /// Ran when bookmarks star is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ShowBookmarks(object sender, RoutedEventArgs e)
    {
        if (sender is not Button Button) {return;}
        UpdateButtons(Button);
        ShellVM.Articles.Clear();
        ShellVM.Offset = 0;
        ShellVM.LoadMoreVisibility= Visibility.Collapsed;
        ShellVM.Articles.AddRange(Pref.BookmarkedArticles);
    }
    
    private async void FilterSource(object sender, RoutedEventArgs e) => await SetPublisherFilter();
    
    private void OpenArticle(object sender, RoutedEventArgs e)
    {
        ShellVM.OpenArticle((((sender as Button)!.DataContext) as Article)!);
    }
        
    private void OpenSearch(object sender, RoutedEventArgs e)
    {
        //Toggle search box visibility
        if (SearchBox.Visibility == Visibility.Collapsed)
        {
            SearchBox.Visibility = Visibility.Visible;
            SearchButton.Foreground = Themer.MainBrush;
            SearchButton.Background = Themer.SecondaryBrush;
        }
        else
        {
            SearchBox.Visibility = Visibility.Collapsed;
            SearchButton.Foreground = Themer.SecondaryBrush;
            SearchButton.Background = Themer.MainBrush;
        }
    }
    
    private async void Search(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ShellVM.Articles.Clear();
        UpdateButtons(new());
        var articles = await ShellVM.Hallon.Search(SearchBox.Text);
        ShellVM.Articles.AddRange(articles);
    }
}
