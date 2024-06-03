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
    public delegate void UpdateButtons(Button Button);
    
    public UpdateButtons UpdateButtonsDelegate;
    public ObservableCollection<Article> Articles { get; set; }
    public AsyncCommand LoadPublicationDataCommand { get; set; }
    public AsyncCommand LoadArticleDataCommand { get; set; }
    
    public ObservableCollection<Filters> Filters =
    [
        new("Latest", ""),
        new("Headlines", "HEADLINE = 1"),
        new("Today", "DATE(Publish_Date) = CURDATE()"),
        new("Business", "SECTORS NOT LIKE ''"),
        new("Elections", "title LIKE '%election%'\r\nOR ARTICLE_TEXT LIKE '%election%'")
    ];

    //Access to the hallon API server
    public API Hallon = new();

    public ShellVM()
    {
        LoadPublicationDataCommand = new AsyncCommand(LoadPublicationData);
        LoadArticleDataCommand = new AsyncCommand(LoadArticleData);
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
    
    /// <summary>
    /// How far we are into the category
    /// </summary>
    public int Offset = 0;

    public async Task LoadArticleData()
    {
        try
        {
            Articles.Clear(); //Reset collection
            string filter;
            if (FilterBy != "" && FilterExtension != "") { filter = $"WHERE {FilterBy} AND {FilterExtension} {FilterOrder}"; }
            else if (FilterExtension != "" && FilterBy == "") { filter = $"WHERE {FilterExtension} {FilterOrder}"; }
            else if (FilterBy != "") {filter = $"WHERE {FilterBy} {FilterOrder}"; }
            else { filter = FilterOrder; }
            

            //Load articles and filter articles from the future.
            Articles.AddRange((await Hallon.GetArticles(Glob.Model.ArticleFetchLimit,
                    Offset, filter))
                .Where(x => DateTime.Now.AddHours(3) > x.PublishDate));
            Offset += Articles.Count;
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

    public async Task LoadPublicationData() => await Hallon.GetPublications();

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
