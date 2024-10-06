using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.UI;
using FirehoseApp.UI.Controls;
using HYDRANT;
using HYDRANT.Definitions;
using NLog;
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
            Data Data = await Hallon.GetData();

            if (Data.MinimumVersion > Data.ApiVersion)
            {
                ShowServerError(
                    new Exception("This version of Firehose is outdated. Please update to the latest version."));
                return;
            }

            //Load Filters and Publications
            Glob.Publications = Data.Publications;
            Filters = new();
            filters.AddRange(Data.Filters);

            await LoadArticleData();
        }
        catch (Exception ex)
        {
            ShowServerError(ex," (/Data endpoint Exception)");
        }
    }

    public async Task LoadArticleData()
    {
        PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

        try
        {
            Articles.Clear(); //Reset collection
            

            //Handle filters
            string Pubs = string.Join(",", PublisherIDs.Select(Publication => Publication.ID.ToString()));
            
            //Load articles
            List<Article> a = await Hallon.GetArticles(Pref.ArticleFetchLimit,
                Offset, CurrentFilter, SearchText,Pubs);

            //Filter by keywords
            var CurrentArticles = a
                .Where(article => !Pref.BlockedKeywords.Any(keyword =>
                    article.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (article.Content ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)));

            //Filter by publisher
            CurrentArticles = CurrentArticles.Where(Article => !Pref.BlockedSources.Contains(Article.Publisher));    

            Articles.AddRange(CurrentArticles);
            Offset += Articles.Count;
        }
        catch (Exception ex)
        {
            ShowServerError(ex);
        }
    }

    /// <summary>
    /// Opens an article.
    /// </summary>
    /// <param name="Article">Hydrant Article object</param>
    public async void OpenArticle(Article Article)
    {
        //Update user stats
        var Pref = Ioc.Default.GetRequiredService<PreferencesModel>();
        Pref.ArticlesOpened++;
        Pref.TimeSaved -= Article.ReadTime;

        await Hallon.AddView(Article.URL);
        CurrentArticle = Article;
        switch (Ioc.Default.GetRequiredService<PreferencesModel>().OpenInMode)
        {
            case 0: //Open in Article WebView
                App.UI.Navigate(typeof(ArticleWebView));
                break;
            case 1: //Open in reader mode
                App.UI.Navigate(typeof(ReaderMode));
                break;
            case 2: //Open in Web Browser
                Windows.System.Launcher.LaunchUriAsync(new Uri(Article.URL));
                break;
        }
    }
    
    /// <summary>
    /// Shows that there's an issue with the server and gracefully closes FHN
    /// </summary>
    /// <param name="Error"></param>
    public async void ShowServerError(Exception Error, string ex = "")
    {
        App.MainWindow.Content = new StackPanel();
        await Glob.OpenContentDialog(new ContentDialog
        {
            Title = "Firehose Server Error" + ex,
            Width = 2000,
            Height = 2000,
            Content = $"""
                       Failed to connect to the server, this might be due  a network issue or the server is down.
                       Check your network connection and try again.
                       
                       Error: {Error.Message}
                       Stack Trace: {Error.StackTrace}
                       """,
            PrimaryButtonText = "Close Firehose",
        });
        
        App.MainWindow.Close(); //Close firehose
    }
}
