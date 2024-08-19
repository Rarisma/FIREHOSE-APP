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
            ShellVM.PublisherID = ((CD.Content as PublisherFilter)
                .Content as ListView)!.SelectedIndex + 1;
        }
        else
        {
            //Set back to Filter button
            ShellVM.PublisherID = -1;
        }
        ShellVM.Offset = 0;
        UpdateButtons(ShellVM.UIFilters.First());
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
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        App.UI.Navigate(typeof(Preferences));
    }
    
    /// <summary>
    /// Updates UI on top row of buttons.
    /// </summary>
    /// <param name="Button">Button to set as selected.</param>
    public void UpdateButtons(FilterButton Button)
    {
        //Show messages
        ShellVM.BoomarksMessageVisibility = Visibility.Collapsed;
        ShellVM.LoadMoreVisibility = Visibility.Visible;
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
        UpdateButtons(ShellVM.UIFilters.First());
        var articles = await ShellVM.Hallon.Search(SearchBox.Text);
        ShellVM.Articles.AddRange(articles);
    }
    
    private void ChangeFilter(object sender, RoutedEventArgs e)
    {
        var button = sender as FilterButton;
        ShellVM.CurrentFilter = button.Content.ToString();
        foreach (var filters in ShellVM.UIFilters)
        {
            filters.Unset();
        }

        button.Set();
        ShellVM.LoadArticleDataCommand.Execute(null);
    }
}
