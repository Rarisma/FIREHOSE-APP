using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FirehoseNews.UI.Controls;
using FirehoseNews.UI;
using FirehoseNews.UI.Dialogs;
using HYDRANT.Definitions;
using Uno.Extensions;

namespace FirehoseNews.Viewmodels;

class ShellVM : ObservableObject
{
    public ObservableCollection<Article> Articles { get; set; }

    public LoadableCommand LoadPublicationDataCommand { get; }
    public LoadableCommand LoadArticleDataCommand { get; }
    
    public ObservableCollection<(string, string)> Filters = new()
    {
        new("Headlines", "HEADLINE = 1"),
        new("Latest", ""),
        new("Today", "BUSINESS_RELATED = 1"),
        new("Business", "BUSINESS_RELATED = 1"),
    };
    private API Hallon = new(Glob.APIKEY);

    public ShellVM()
    {
        LoadPublicationDataCommand = new LoadableCommand(LoadPublicationData);
        LoadArticleDataCommand = new LoadableCommand(LoadArticleData);
    }
    
    /// <summary>
    /// Extra filter for use while filtering publications
    /// </summary>
    public string FilterBy = "";
    public string FilterExtension = "";
    public string FilterOrder = "";
    
    /// <summary>
    /// ID of publisher filter to filter for
    /// </summary>
    public int PublisherID = -1;
    
    public int Offset = 0;

    async Task LoadArticleData()
    {
        try
        {
            Offset += Articles.Count;
            Articles.Clear(); //Reset collection
            string filter;
            if (FilterBy != "")
            {
                filter = $"WHERE {FilterBy} {FilterExtension} {FilterOrder}";
            }
            else { filter = FilterOrder; }
            

            //Load articles and filter articles from the future.
            Articles.AddRange((await new API(Glob.APIKEY).GetArticles(Glob.Model.ArticleFetchLimit,
                    Offset, filter))
                .Where(x => DateTime.Now.AddHours(3) > x.PublishDate));
        }
        catch (Exception ex)
        {
            Articles.Add(new()
            {
                Title = "Failed to load articles!",
                RSSSummary = ex.Message,
                PublisherID = 0,
                ImageURL = "?",
                PublishDate = new DateTime(1911, 11, 19),
            });
            Console.WriteLine(ex.ToString());
        }
    }

    async Task LoadPublicationData() => await Hallon.GetPublications();

    public async void OpenArticle(Article article, bool IgnoreQuickView = false)
    {
        //Open the summary if the article has been tapped and the user
        //has enabled tap opens summary
        if (Glob.Model.ArticleTapOpensSummary && IgnoreQuickView == false) 
        {
            await OpenQuickView(article);
        }
        else if (Glob.Model.AlwaysOpenReader)
        {
            Glob.NaviStack.Push(new ReaderMode(article));
        }
        else
        {
            Glob.NaviStack.Push(new ArticleView(article));
        }
        
        //Navigate to view
        Glob.DoNavi();
    }
    
    public async Task OpenQuickView(Article article)
    {
        // Create content dialog
        ContentDialog CD = new()
        {
            Content = new QuickView(article),
            XamlRoot = Glob.XamlRoot,
            Title = $"Summary for {article.Title}",
            PrimaryButtonText = "Open Article",
            SecondaryButtonText = "Close"
        };
        
        // Perform actions based on dialog result
        if (await CD.ShowAsync() == ContentDialogResult.Primary)
        {
            OpenArticle(article, true);
        }
    }

}
