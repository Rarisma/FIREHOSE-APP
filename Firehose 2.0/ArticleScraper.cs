using System.Collections.Concurrent;
using System.ServiceModel.Syndication;
using Firehose;
using Firehose2.Stocks;
using HtmlAgilityPack;
using HYDRANT;
using OpenGraphNet;

namespace Firehose2;
internal static class ArticleScraper
{
    public static ConcurrentBag<Article> ProcessedArticles = new();
    public static void StartScraping()
    {
        while (true)
        {
            var keys = ArticleDiscovery.Pending.Keys.ToList();
            foreach (var key in keys)
            {
                if (ArticleDiscovery.Pending.TryGetValue(key, out (SyndicationItem, int) Item))
                {
                    ArticleDiscovery.Pending.Remove(key);
                    CreateArticle(Item);
                }
            }

            Thread.Sleep(10000);
        }
    }

    private static async void CreateArticle((SyndicationItem, int) Article)
    {
        string URL = Article.Item1.Links.FirstOrDefault()?.Uri?.ToString();

        string Img;
        try
        {
            //Get Open Graph Data
            var data = await OpenGraph.ParseUrlAsync(URL);

            //Set image if found.
            if (data.Image != null) { Img = data.Image.ToString(); }
            else { Img = "?";}
        }
        catch (Exception e) {Img = "?";}

        string ArticleText;
        try { ArticleText = await StringTools.GetURLText(URL); }
        catch { return; }

        try
        {
            ProcessedArticles.Add(new Article
            {
                Url = URL,
                ArticleText = ArticleText,
                Title = Article.Item1.Title.Text,
                RSSSummary = StringTools.CleanUpHTMLString(Article.Item1.Summary?.Text),
                PublishDate = Article.Item1.PublishDate.DateTime,
                Paywall = StringTools.IsPaywalled(URL),
                Summary = "",
                PublisherID = Article.Item2,
                ImageURL = Img,
                CompaniesMentioned = Company.FindCompaniesInText(ArticleText, CompanyData.Tickers)
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
