using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Firehose.UI;
using Firehose.UI.Controls;
using HYDRANT;
using HYDRANT.Definitions;
using Uno.Extensions;

namespace Firehose.Viewmodels;

class ShellVM : ObservableObject
{
    public object Content { get; set; }

    public delegate void UpdateButtons(Button Button);
    
    public UpdateButtons UpdateButtonsDelegate;
    public ObservableCollection<Article> Articles { get; set; }
    public AsyncCommand LoadPublicationDataCommand { get; set; }
    public AsyncCommand LoadArticleDataCommand { get; set; }

    public string NoBookmarksText { get; set; }
    public Visibility LoadMoreVisibility { get; set; }
    public SolidColorBrush BookmarksButtonForeground { get; set; }
    public SolidColorBrush BookmarksButtonBackground { get; set; }

    public ObservableCollection<Filters> Filters =
    [
        new("Latest", ""),
        new("Headlines", "IMPACT > 70"),
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
        NoBookmarksText = "You have no bookmarked stories.";
        Articles = new();
        LoadMoreVisibility = Visibility.Visible;
        Content = new Frame();
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
                Summary = ex.Message,
                PublisherID = 0,
                ImageURL = "?",
                PublishDate = new DateTime(1911, 11, 19),
            });
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task LoadPublicationData() => await Hallon.GetPublications();

    public void OpenArticle(Article article)
    {
        //Open article in reader mode if always open reader is enabled
        if (Glob.Model.AlwaysOpenReader)
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
}
