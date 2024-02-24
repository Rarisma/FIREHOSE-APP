//I ask: Why did humans disappear from the world?
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Firehose;
/// <summary>
/// Subjugates TNC/TNS from Scott/Lewis
/// </summary>
static class SubjugationService
{
    private static string[] Blacklist = File.ReadAllLines("Blacklist.txt");
    private const string Proxy = "https://1ft.io/";

    private static List<string> Paywalled = new(){"www.nytimes.com", "https://www.ft.com/" };
    
    /// <summary>
    /// Scrapes web pages
    /// </summary>
    /// <param name="url">URL to scrape</param>
    /// <returns>Returns text</returns>
    public static async Task<string> GetURLText(string url)
    {
        if (Paywalled.Any(s => url.Contains(s)))
        {
            url = Proxy + url;
        }
        
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.3");

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            HtmlDocument document = new();
            document.LoadHtml(content);
            var text = document.DocumentNode.InnerText;
            return CleanText(text);
        }
    }
    
    /// <summary>
    /// Cleans up article text
    /// </summary>
    /// <param name="text">Text to clean</param>
    /// <returns>Cleaned article text</returns>

    public static string CleanText(string text)
    {
        text = Regex.Replace(text, @"\n+", "\n");
        var strippedLines = text.Split('\n').Select(line => line.TrimEnd());
        text = string.Join("\n", strippedLines);
        text = Regex.Replace(text, @"\s+", " ");
        foreach (var item in Blacklist)
        {
            text = text.Replace(item, "");
        }
        return text;
    }

}