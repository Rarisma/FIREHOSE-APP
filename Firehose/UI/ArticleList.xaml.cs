using CommunityToolkit.Mvvm.DependencyInjection;
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
        ShellVM.UpdateButtonsDelegate = UpdateButtons;
        ShellVM.Articles = new();
        InitializeComponent();
        ChangeFilter(ShellVM.Filters[0], new());
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
        ShellVM.NoBookmarksText = " ";

        //Show load more button
        ShellVM.LoadMoreVisibility = Visibility.Visible;

        //Clear filter buttons
        foreach (var FilterButton in ShellVM.Filters)
        {
            FilterButton.Background = new SolidColorBrush(Colors.Transparent);
            FilterButton.Foreground = Themer.SecondaryBrush;
        }
        //Bookmarks button isn't in the filters stack panel so clear them manually.
        ShellVM.BookmarksButtonBackground = new SolidColorBrush(Colors.Transparent);
        ShellVM.BookmarksButtonForeground = Themer.SecondaryBrush;
        
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
        ShellVM.LoadMoreVisibility = Visibility.Visible;
        UpdateButtons((Button)sender);

        //Set correct filter
        ShellVM.LoadArticleDataCommand.Execute(null);
    }

    private void ArticleList_OnLoaded(object sender, RoutedEventArgs e) => Glob.XamlRoot = XamlRoot;
    
    private void ShowBookmarks(object sender, RoutedEventArgs e)
    {
        if (sender is not Button Button) {return;}
        UpdateButtons(Button);
        ShellVM.Articles.Clear();
        ShellVM.Offset = 0;
        ShellVM.LoadMoreVisibility= Visibility.Collapsed;
        ShellVM.Articles.AddRange(Glob.Model.BookmarkedArticles);

        //Show no bookmarks text, so it's not a blank screen
        if (Glob.Model.BookmarkedArticles.Count == 0)
        {
            ShellVM.NoBookmarksText = "You have no bookmarked articles.";
        }
    }
    
    private async void FilterSource(object sender, RoutedEventArgs e) => await SetPublisherFilter();
    
    private void OpenArticle(object sender, RoutedEventArgs e)
    {
        ShellVM.OpenArticle((((sender as Button)!.DataContext) as Article)!);
    }
}
