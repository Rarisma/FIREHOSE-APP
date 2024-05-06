using System.Text.RegularExpressions;
using HtmlAgilityPack;
//
namespace Firehose;
/// <summary>
/// IAI Port of TNC, TNS and various tools.
/// </summary>
static class StringTools
{
    private static string[] Blacklist = File.ReadAllLines("Data/Blacklist.txt");
    private const string Proxy = "https://1ft.io/";

    private static List<string> Paywalled = new(){ "https://www.nytimes.com", "https://www.ft.com/" };

    public static bool IsPaywalled(string URL) => Paywalled.Any(URL.Contains);

    /// <summary>
    /// Scrapes web pages
    /// </summary>
    /// <param name="url">URL to scrape</param>
    /// <returns>Returns text</returns>
    public static async Task<string> GetURLText(string url)
    {
        if (Paywalled.Any(s => url.Contains(s))) { url = Proxy + url; }
        
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.3");

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            HtmlDocument document = new();
            document.LoadHtml(content);
            return CleanText(document.DocumentNode.InnerText);
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

    public static string MarkdownToPlainText(string markdownText)
    {
        // Remove headers
        string noHeaders = Regex.Replace(markdownText, @"(#+\s)", "");

        // Remove bold and italics
        string noBoldItalics = Regex.Replace(noHeaders, @"(\*{1,2}|_{1,2})", "");

        // Replace links with link text only
        string noLinks = Regex.Replace(noBoldItalics, @"\[(.*?)\]\(.*?\)", "$1");

        // Remove images
        string noImages = Regex.Replace(noLinks, @"\!\[(.*?)\]\(.*?\)", "");

        // Remove HTML tags
        string noHtml = Regex.Replace(noImages, @"<[^>]*>", "");

        // Replace lists with plain text
        string noLists = Regex.Replace(noHtml, @"^\s*([-*]|\d+\.)\s+", "", RegexOptions.Multiline);

        // Remove additional Markdown elements as necessary
        // Example: Remove code blocks
        string noCodeBlocks = Regex.Replace(noLists, @"(```.*?```|`.*?`)", "");

        return noCodeBlocks;
    }


    /// <summary>
    /// Takes a block of HTML Text, like a rich RSS Summary
    /// and turns it into pretty plain ol' plaintext.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string CleanUpHTMLString(string Text)
    {
        try
        {
            // Load the HTML content
            HtmlDocument doc = new();
            doc.LoadHtml(Text);

            // Extract plain text and return it
            return doc.DocumentNode.InnerText;
        }
        catch { return " "; }
    }
}
