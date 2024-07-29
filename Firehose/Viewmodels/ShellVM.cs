using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.UI;
using FirehoseApp.UI.Controls;
using HtmlAgilityPack;
using HYDRANT;
using HYDRANT.Definitions;
using Uno.Extensions;

namespace FirehoseApp.Viewmodels;

class ShellVM : ObservableObject
{
    public object Content { get; set; }

    public delegate void UpdateButtons(Button Button);
    
    public UpdateButtons? UpdateButtonsDelegate;
    public ObservableCollection<Article> Articles { get; set; }
    public AsyncCommand LoadAllDataCommand { get; set; }
    public AsyncCommand LoadArticleDataCommand { get; set; }

    public string NoBookmarksText { get; set; }
    public Visibility LoadMoreVisibility { get; set; }
    public SolidColorBrush BookmarksButtonForeground { get; set; }
    public SolidColorBrush BookmarksButtonBackground { get; set; }

    public ObservableCollection<Filters> Filters;

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
        foreach (var filter in await new API().GetFilters(Glob.Model.AccountToken))
        {
            Ioc.Default.GetRequiredService<ShellVM>().Filters.Add(new(filter));
        }
    }
    public async Task LoadArticleData()
    {
        try
        {
            Articles.Clear(); //Reset collection

            //Load articles and filter articles from the future.
            List<Article> a = await Hallon.GetArticles(Glob.Model.ArticleFetchLimit,
                Offset, Glob.Model.AccountToken, CurrentFilter, true, PublisherID);
            Articles.AddRange(a.Where(x => DateTime.Now.AddHours(3) > x.PublishDate));
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
        switch (Glob.Model.OpenInMode)
        {
            case 0: //Open in Article WebView
                Glob.DoNavi(new ArticleView(article));
                break;
            case 1: //Open in reader mode
                if (string.IsNullOrEmpty(article.Text))
                {
                    article.Text = await new API().GetArticleText(article.Url);
                }
                Glob.DoNavi(new ReaderMode(article));
                break;
            case 2: //Open in Web Browser
                Windows.System.Launcher.LaunchUriAsync(new Uri(article.Url));
                break;
        }
    }
}
