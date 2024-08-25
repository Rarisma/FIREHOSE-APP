using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.UI;
using FirehoseApp.UI.Controls;
using HYDRANT;
using HYDRANT.Definitions;
using Uno.Extensions;
//WORLDS APART
namespace FirehoseApp.Viewmodels;
public class ShellVM : ObservableObject
{
    #region Properties
    private ObservableCollection<Article> _articles;
    
    public ObservableCollection<Article> Articles
    {
        get => _articles;
        set => SetProperty(ref _articles, value);
    }
    
    private AsyncCommand _loadAllDataCommand;
    /// <summary>
    /// Loads filter buttons and Publication data.
    /// </summary>
    public AsyncCommand LoadAllDataCommand
    {
        get => _loadAllDataCommand;
        set => SetProperty(ref _loadAllDataCommand, value);
    }
    
    private AsyncCommand _loadArticleDataCommand;
    
    public AsyncCommand LoadArticleDataCommand
    {
        get => _loadArticleDataCommand;
        set => SetProperty(ref _loadArticleDataCommand, value);
    }
    
    private AsyncCommand _SearchCommand;
    
    public AsyncCommand SearchCommand
    {
        get => _SearchCommand;
        set => SetProperty(ref _SearchCommand, value);
    }
    private Visibility _loadMoreVisibility;
    
    public Visibility LoadMoreVisibility
    {
        get => _loadMoreVisibility;
        set => SetProperty(ref _loadMoreVisibility, value);
    }
    
    private Visibility _BoomarksMessageVisibility;
    
    public Visibility BoomarksMessageVisibility
    {
        get => _BoomarksMessageVisibility;
        set => SetProperty(ref _BoomarksMessageVisibility, value);
    }
    
    private ObservableCollection<Filter> filters;
    
    public ObservableCollection<Filter> Filters
    {
        get => filters;
        set => SetProperty(ref filters, value);
    }
    
    private ObservableCollection<FilterButton> _UIFilters;
    
    public ObservableCollection<FilterButton> UIFilters
    {
        get => _UIFilters;
        set => SetProperty(ref _UIFilters, value);
    }

    //Access to the Hydrant API server
    public API Hallon = new();
    
    
    public string CurrentFilter;
    

    private ObservableCollection<Publication> _PublisherIDs;
    /// <summary>
    /// ID of publisher filter to filter for
    /// </summary>
    public ObservableCollection<Publication> PublisherIDs
    {
        get => _PublisherIDs;
        set => SetProperty(ref _PublisherIDs, value);
    }
    /// <summary>
    /// How far we are into the category
    /// </summary>
    public int Offset;

    private string _searchText;
    
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private Article _CurrentArticle;
    /// <summary>
    /// Currently selected article (for webview, reader etc)
    /// </summary>
    public Article CurrentArticle
    {
        get => _CurrentArticle;
        set => SetProperty(ref _CurrentArticle, value);
    }
    #endregion

    public ShellVM()
    {
        CurrentFilter = "Latest";
        Filters = new();
        LoadAllDataCommand = new AsyncCommand(LoadAllData);
        LoadArticleDataCommand = new AsyncCommand(LoadArticleData);
        Articles = new();
        LoadMoreVisibility = Visibility.Visible;
        SearchCommand = new AsyncCommand(Search);
        BoomarksMessageVisibility = Visibility.Visible;
        PublisherIDs = new();
        Offset = 0;
        UIFilters = new();
    }

    /// <summary>
    /// Loads filter and publication data.
    /// </summary>
    private async Task LoadAllData()
    {
        try
        {
            Glob.Publications = await LoadPublicationData();
            await LoadFilterData();
            await LoadArticleData();
        }
        catch (Exception ex)
        {
            ShowServerError(ex.Message);
        }
    }
    
    private async Task Search()
    {
        Articles.Clear();
        Articles.AddRange(await Hallon.Search(SearchText));
    }

    /// <summary>
    /// Loads all filter data.
    /// </summary>
    public async Task LoadFilterData()
    {
        PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();
        Filters.AddRange(await Hallon.GetFilters(Pref.AccountToken));
    }

    public async Task LoadArticleData()
    {
        PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

        try
        {
            Articles.Clear(); //Reset collection
            
            bool MinimalMode = Pref.BlockedKeywords.Count == 0;

            //Handle filters
            string Pubs = string.Join(",", PublisherIDs.Select(publication => publication.ID.ToString()));
            
            //Load articles
            List<Article> a = await Hallon.GetArticles(Pref.ArticleFetchLimit,
                Offset, Pref.AccountToken, CurrentFilter, MinimalMode, Pubs);

            //filter articles from the future
            //We do this because sometimes an article is published weeks in advanced (Economist worlds apart)
            var CurrentArticles = a.Where(x => DateTime.Now.AddHours(3) > x.PublishDate)
                //Filter by keywords
                .Where(article => !Pref.BlockedKeywords.Any(keyword =>
                    article.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (article.Text ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)));

            //Filter by publisher
            CurrentArticles = CurrentArticles.Where(article => !Pref.BlockedSources.Contains(article.PublisherID));    

            Articles.AddRange(CurrentArticles);
            Offset += Articles.Count;
        }
        catch (Exception ex)
        {
            ShowServerError(ex.Message);
        }
    }

    public async Task<List<Publication>> LoadPublicationData() => await Hallon.GetPublications();

    public async void OpenArticle(Article article)
    {
        await Hallon.AddView(article.Url);
        CurrentArticle = article;
        switch (Ioc.Default.GetRequiredService<PreferencesModel>().OpenInMode)
        {
            case 0: //Open in Article WebView
                App.UI.Navigate(typeof(ArticleWebView));
                break;
            case 1: //Open in reader mode
                if (string.IsNullOrEmpty(article.Text))
                {
                    article.Text = await Hallon.GetArticleText(article.Url);
                }
                App.UI.Navigate(typeof(ReaderMode));

                break;
            case 2: //Open in Web Browser
                Windows.System.Launcher.LaunchUriAsync(new Uri(article.Url));
                break;
        }
    }
    
    /// <summary>
    /// Shows that there's an issue with the server and gracefully closes FHN
    /// </summary>
    /// <param name="Error"></param>
    public async void ShowServerError(string Error)
    {
        App.MainWindow.Content = new StackPanel();
        await Glob.OpenContentDialog(new ContentDialog
        {
            Title = "Firehose is unavailable",
            Content = $"""
                       Failed to connect to the firehose server.
                       This might be due to a network issue or the server being down.
                       
                       Check your network connection and try again.
                       
                       Error: {Error}
                       """,
            PrimaryButtonText = "Close Firehose",
        });
        
        App.MainWindow.Close(); //Close firehose
    }
}
