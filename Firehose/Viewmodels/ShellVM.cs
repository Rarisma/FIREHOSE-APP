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

class ShellVM : ObservableObject
{
    
    public delegate void UpdateButtons(Button button);
    public UpdateButtons UpdateButtonsDelegate;

    
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
    
    private Visibility _loadMoreVisibility;
    
    public Visibility LoadMoreVisibility
    {
        get => _loadMoreVisibility;
        set => SetProperty(ref _loadMoreVisibility, value);
    }
    
    private ObservableCollection<Filters> filters;
    
    public ObservableCollection<Filters> Filters
    {
        get => filters;
        set => SetProperty(ref filters, value);
    }


    //Access to the hallon API server
    public API Hallon = new();

    public ShellVM()
    {
        CurrentFilter = "Latest";
        Filters = new();
        LoadAllDataCommand = new AsyncCommand(LoadAllData);
        LoadArticleDataCommand = new AsyncCommand(LoadArticleData);
        Articles = new();
        LoadMoreVisibility = Visibility.Visible;
        PublisherID = -1;

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
            UpdateButtonsDelegate.Invoke(Filters[0]);
        }
        catch (Exception ex)
        {
            ShowServerError(ex.Message);
        }
    }
    
    
    public string CurrentFilter;
    
    /// <summary>
    /// ID of publisher filter to filter for
    /// </summary>
    public int PublisherID;
    
    /// <summary>
    /// How far we are into the category
    /// </summary>
    public int Offset;
    
    /// <summary>
    /// Loads all filter data.
    /// </summary>
    /// <returns></returns>
    public async Task LoadFilterData()
    {
        PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

        foreach (var filter in await Hallon.GetFilters(Pref.AccountToken))
        {
            Ioc.Default.GetRequiredService<ShellVM>().Filters.Add(new(filter));
        }
    }
    public async Task LoadArticleData()
    {
        PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

        try
        {
            Articles.Clear(); //Reset collection
            
            bool MinimalMode = Pref.BlockedKeywords.Count == 0;
            //Load articles
            List<Article> a = await Hallon.GetArticles(Pref.ArticleFetchLimit,
                Offset, Pref.AccountToken, CurrentFilter, MinimalMode, PublisherID);

            //filter articles from the future
            //We do this because sometimes an article is published weeks in advanced (Economist worlds apart)
            var CurrentArticles = a.Where(x => DateTime.Now.AddHours(3) > x.PublishDate);
            Articles.AddRange(CurrentArticles.Where(article =>
                    !Pref.BlockedKeywords.Any(keyword =>
                        article.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (article.Text ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList());
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
        switch (Ioc.Default.GetRequiredService<PreferencesModel>().OpenInMode)
        {
            case 0: //Open in Article WebView
                Glob.DoNavi(new ArticleView(article));
                break;
            case 1: //Open in reader mode
                if (string.IsNullOrEmpty(article.Text))
                {
                    article.Text = await Hallon.GetArticleText(article.Url);
                }
                Glob.DoNavi(new ReaderMode(article));
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
