using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using Firehose;
using HtmlAgilityPack;
using HYDRANT;

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
        try
        {
            ProcessedArticles.Add(new Article
            {
                Url = Article.Links.FirstOrDefault()?.Uri?.ToString(),
                Title = Article.Title.Text,
                RssSummary = StringTools.CleanUpHTMLString(Article.Summary?.Text),
                Content = await StringTools.GetURLText(Article.Links.FirstOrDefault()?.Uri?.ToString()),
                PublishDate = Article.PublishDate.DateTime,
                Paywall = StringTools.IsPaywalled(Article.Links.FirstOrDefault()?.Uri?.ToString()),
                Summary = "",
                Publisher = Article.Links.FirstOrDefault()?.Uri?.Host.ToLower().Replace("www.", "").Split(".")[0].ToUpper(),
                Author = Article.Authors.Count == 0 ? "Unknown Author" : Article.Authors.First().Name,
                ImageURL = await GetImageForURL(Article.Links.FirstOrDefault()?.Uri?.ToString())
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
        try
        {
            using (var httpClient = new HttpClient())
            {
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
