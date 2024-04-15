using System.ServiceModel.Syndication;
using Firehose;
using HtmlAgilityPack;
using HYDRANT;
using OpenGraphNet;

namespace Firehose2;
internal static class ArticleScraper
{
    public static List<Article> ProcessedArticles = new ();
    public static void StartScraping()
    {
        while (true)
        {
            var keys = ArticleDiscovery.Pending.Keys.ToList();
            foreach (var key in keys)
            {
                if (ArticleDiscovery.Pending.TryGetValue(key, out SyndicationItem art))
                {
                    ArticleDiscovery.Pending.Remove(key);
                    CreateArticle(art);
                }
            }

            keys = ArticleDiscovery.PendingLowPriority.Keys.ToList();
            foreach (var key in keys)
            {
                if (ArticleDiscovery.PendingLowPriority.TryGetValue(key, out SyndicationItem art))
                {
                    ArticleDiscovery.PendingLowPriority.Remove(key);
                    CreateArticle(art);
                }
            }

            Thread.Sleep(10000);
        }
    }

    private static async void CreateArticle(SyndicationItem Article)
    {
        string URL = Article.Links.FirstOrDefault()?.Uri?.ToString();

        string Img;
        try { Img = (await OpenGraph.ParseUrlAsync(URL)).Image.ToString(); }
        catch {Img = "?";}

        string ArticleText;
        try { ArticleText = await StringTools.GetURLText(URL); }
        catch { return; }

        try
        {
            ProcessedArticles.Add(new Article
            {
                Url = URL,
                Content = ArticleText,
                Title = Article.Title.Text,
                RssSummary = StringTools.CleanUpHTMLString(Article.Summary?.Text),
                PublishDate = Article.PublishDate.DateTime,
                Paywall = StringTools.IsPaywalled(URL),
                Summary = "",
                Publisher = URL.Replace("www.", "").Split(".")[0].ToUpper().ToString().Replace("HTTPS://", ""),
                Author = Article.Authors.Count == 0 ? "Unknown Author" : Article.Authors.First().Name,
                ImageURL = Img
            });
        }
        catch { }
    }

    /// <summary>
    /// Gets the image for a URL from the OpenGraph Metadata
    /// </summary>
    /// <param name="URL">URL to scrape</param>
    /// <returns>OpenGraph Image URL</returns>
    public static async Task<string> GetImageForURL(string URL)
    {
        //NY Times doesn't do OpenGraph Stuf.
        if (URL.Contains("www.nytimes.com")) { return "?";}


        try
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.3");

                HtmlDocument htmlDoc = new();
                htmlDoc.LoadHtml(await httpClient.GetStringAsync(URL));

                var ogImageNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                if (ogImageNode != null)
                {
                    return ogImageNode.GetAttributeValue("content", string.Empty);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"(Image Grabber - URL {URL}) Error: {e.Message}");
        }
        return "?";
    }

}
