using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.UI;
using FirehoseApp.UI.Controls;
using HtmlAgilityPack;
using HYDRANT;
using HYDRANT.Definitions;
using Uno.Extensions;

namespace FirehoseApp.Viewmodels;

class ShellVM : ObservableObject
{
    private object content;
    
    public object Content
    {
        get => content;
        set => SetProperty(ref content, value);
    }
    
    public delegate void UpdateButtons(Button button);
    
    private UpdateButtons? updateButtonsDelegate;
    
    public UpdateButtons? UpdateButtonsDelegate
    {
        get => updateButtonsDelegate;
        set => SetProperty(ref updateButtonsDelegate, value);
    }
    
    private ObservableCollection<Article> articles;
    
    public ObservableCollection<Article> Articles
    {
        get => articles;
        set => SetProperty(ref articles, value);
    }
    
    private AsyncCommand loadAllDataCommand;
    
    public AsyncCommand LoadAllDataCommand
    {
        get => loadAllDataCommand;
        set => SetProperty(ref loadAllDataCommand, value);
    }
    
    private AsyncCommand loadArticleDataCommand;
    
    public AsyncCommand LoadArticleDataCommand
    {
        get => loadArticleDataCommand;
        set => SetProperty(ref loadArticleDataCommand, value);
    }
    
    private string noBookmarksText;
    
    public string NoBookmarksText
    {
        get => noBookmarksText;
        set => SetProperty(ref noBookmarksText, value);
    }
    
    private Visibility loadMoreVisibility;
    
    public Visibility LoadMoreVisibility
    {
        get => loadMoreVisibility;
        set => SetProperty(ref loadMoreVisibility, value);
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
        LoadAllDataCommand = new AsyncCommand((async () =>
        {
            Glob.Publications = await LoadPublicationData();
            await LoadFilterData();
            await LoadArticleData();
            UpdateButtonsDelegate.Invoke(Filters[0]);
        }));
        LoadArticleDataCommand = new AsyncCommand(LoadArticleData);
        NoBookmarksText = "You have no bookmarked stories.";
        Articles = new();
        LoadMoreVisibility = Visibility.Visible;
        Content = new Frame();
    }
    
    
    public string CurrentFilter = "";

    /// <summary>
    /// ID of publisher filter to filter for
    /// </summary>
    public int PublisherID = -1;
    
    /// <summary>
    /// How far we are into the category
    /// </summary>
    public int Offset = 0;
    
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
                        article.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList());
            Offset += Articles.Count;
        }
        catch (Exception ex)
        {
            Articles.Add(new()
            {
                Title = "Failed to load articles!",
                Summary = ex.Message,
                PublisherID = 0,
                ImageURL = "?",
                PublishDate = new DateTime(1911, 11, 19),
            });
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task<List<Publication>> LoadPublicationData() => await Hallon.GetPublications();

    public async void OpenArticle(Article article)
    {
        switch (Ioc.Default.GetRequiredService<PreferencesModel>().OpenInMode)
        {
            case 0: //Open in Article WebView
                Glob.DoNavi(new ArticleView(article));
                break;
            case 1: //Open in reader mode
                if (string.IsNullOrEmpty(article.Text))
                {
                    article.Text = await  Ioc.Default.GetRequiredService<ShellVM>()
                        .Hallon.GetArticleText(article.Url);
                }
                Glob.DoNavi(new ReaderMode(article));
                break;
            case 2: //Open in Web Browser
                Windows.System.Launcher.LaunchUriAsync(new Uri(article.Url));
                break;
        }
    }
}
