using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.UI.Controls;
using FirehoseApp.Viewmodels;
using HYDRANT.Definitions;
using Uno.Extensions;

namespace FirehoseApp.UI;

[ObservableObject]
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
            Title = "Filter by publisher",
            Content = new PublisherFilter(),
            PrimaryButtonText = "Filter by Publisher",
            SecondaryButtonText = "Clear publisher filter",
        };
        
        if (await Glob.OpenContentDialog(CD) == ContentDialogResult.Primary)
        {
            ShellVM.PublisherIDs.Clear();
            foreach (Publication pub in (((CD.Content as PublisherFilter)
                         .Content as Grid).Children[1] as ListView)!.SelectedItems)
            {
                ShellVM.PublisherIDs.Add(pub);
                FilterButton.IsChecked = true;
            }
        }
        else
        {
            //Set back to Filter button
            ShellVM.PublisherIDs = new();
            FilterButton.IsChecked = false;
        }
        ShellVM.Offset = 0;
        UpdateButtons();
        ShellVM.LoadArticleDataCommand.Execute(null);
    }
    

    public ArticleList()
    {
        //Init UI if needed
        if (ShellVM.Filters.Count == 0)
        {
            ShellVM.LoadAllDataCommand.Execute(null);
            ShellVM.LoadMoreVisibility = Visibility.Visible;
        }
        
        InitializeComponent();
        UpdateButtons();
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        Settings.IsChecked = false;
        App.UI.Navigate(typeof(Preferences));
    }
    
    /// <summary>
    /// Updates UI on top row of buttons.
    /// </summary>
    public void UpdateButtons()
    {
        //Show messages
        ShellVM.BoomarksMessageVisibility = Visibility.Collapsed;
        ShellVM.LoadMoreVisibility = Visibility.Visible;
        
        BookmarksButton.IsChecked = false;
    }

    private void ArticleList_OnLoaded(object sender, RoutedEventArgs e) => Glob.XamlRoot = XamlRoot;
    
    /// <summary>
    /// Ran when bookmarks star is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ShowBookmarks(object sender, RoutedEventArgs e)
    {
        ShellVM.Articles.Clear();
        ShellVM.Offset = 0;
        ShellVM.LoadMoreVisibility= Visibility.Collapsed;
        ShellVM.BoomarksMessageVisibility = Visibility.Visible; 
        BookmarksButton.Background = Themer.MainBrush;
        BookmarksButton.Foreground = Themer.SecondaryBrush;
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
    
    private void Search(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ShellVM.SearchCommand.Execute(null);
    }
    
    private void ChangeFilter(object sender, RoutedEventArgs e)
    {
        var button = sender as FilterButton;

        //If filter changed reset offset
        if (button.Content.ToString() != ShellVM.CurrentFilter)
        {
            ShellVM.CurrentFilter = button.Content.ToString();
            ShellVM.Offset = 0;
        }

        foreach (var filters in ShellVM.UIFilters)
        {
            filters.Unset();
        }

        button.Set();
        ShellVM.LoadArticleDataCommand.Execute(null);
        UpdateButtons();
    }
    
    /// <summary>
    /// Refreshes new articles
    /// </summary>
    private void Refresh(RefreshContainer sender, RefreshRequestedEventArgs args)
    {
        //Fetch latest articles, then refresh
        ShellVM.Offset = 0;
        ShellVM.LoadArticleDataCommand.Execute(null);
    }
    
    private async void InfScrollCheck(object sender, ScrollViewerViewChangedEventArgs e)
    {
        var verticalOffset = ArticleContainer.VerticalOffset;
        var maxVerticalOffset = ArticleContainer.ScrollableHeight; 
        
        if (maxVerticalOffset >= 0 && verticalOffset >= maxVerticalOffset - 100) // Adjust threshold as needed
        {
            // Trigger loading more items
            ShellVM.LoadArticleDataCommand.Execute(null);
        }
    }
    
    private void ShowSummary(Expander Sender, ExpanderExpandingEventArgs Args)
    {
        var Article = Sender.DataContext as Article;
        
        
        // Add or Remove time saved depending on if expander is opening or closing
        Pref.TimeSaved += Sender.IsExpanded ? Article.ReadTime : Article.ReadTime*-1;
    }
}
