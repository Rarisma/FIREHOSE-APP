using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FirehoseApp.UI;
using FirehoseApp.UI.Controls;
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
    public AsyncCommand LoadPublicationDataCommand { get; set; }
    public AsyncCommand LoadArticleDataCommand { get; set; }

    public string NoBookmarksText { get; set; }
    public Visibility LoadMoreVisibility { get; set; }
    public SolidColorBrush BookmarksButtonForeground { get; set; }
    public SolidColorBrush BookmarksButtonBackground { get; set; }

    public ObservableCollection<Filters> Filters =
    [
        new("Latest", ""),
        new("Headlines", "IMPACT > 70 AND DATE(Publish_Date) >= DATE_SUB(CURDATE(), INTERVAL 4 DAY)","ORDER BY IMPACT DESC"),
        new("Today", "DATE(Publish_Date) = CURDATE()","ORDER BY IMPACT DESC"),
        new("Business", "SECTORS NOT LIKE '' AND DATE(Publish_Date) = CURDATE()","ORDER BY IMPACT DESC"),
        new("Elections", "title LIKE '%election%'\r\nOR ARTICLE_TEXT LIKE '%election%' " +
                         " AND DATE(Publish_Date) >= DATE_SUB(CURDATE(), INTERVAL 4 DAY)","ORDER BY IMPACT DESC")
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
