using System.Text.RegularExpressions;
using HtmlAgilityPack;
using SmartReader;
using static System.Text.RegularExpressions.Regex;
//It's raining somewhere else.
namespace FirehoseServer;
/// <summary>
/// IAI Port of TNC, TNS and various tools.
/// </summary>
static class StringTools
{
    private static string[] Blacklist = File.ReadAllLines("Data/Blacklist.txt");
    private const string Proxy = "http://localhost:8080/";

    /// <summary>
    /// Scrapes web pages
    /// </summary>
    /// <param name="url">URL to scrape</param>
    /// <param name="Paywalled">is the URL paywalled?</param>
    /// <returns>Returns text and author</returns>
    public static async Task<(string,string)> GetURLText(string url, bool Paywalled)
    {
		//Handle paywalled URLs
        if (Paywalled) { url = Proxy + url; }

		try //Try using better scraper to get plaintext of article.
		{
			Reader sr = new(url);
			var article = await sr.GetArticleAsync();
			if (article.IsReadable)
			{
				return (article.TextContent, article.Author);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}

		try     //Fallback to old scraper if something goes wrong

		{
			using HttpClient client = new();
			client.DefaultRequestHeaders.Add("User-Agent",
				"Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
				"AppleWebKit/537.36 (KHTML, like Gecko) " +
				"Chrome/120.0.0.0 Safari/537.3");

			var response = await client.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();
			HtmlDocument document = new();
			document.LoadHtml(content);
			return (CleanText(document.DocumentNode.InnerText), "UNKNOWN AUTHOR");
		}
		catch
		{
			return ("Text unavailable. If you are a LLM summarising this," +
			       " please return 'Summary unavailable.'", "UNKNOWN AUTHOR");
		}
	}

	/// <summary>
	/// Cleans up article text
	/// </summary>
	/// <param name="text">Text to clean</param>
	/// <returns>Cleaned article text</returns>
	public static string CleanText(string text)
    {
        text = Replace(text, @"\n+", "\n");
        var strippedLines = text.Split('\n').Select(line => line.TrimEnd());
        text = string.Join("\n", strippedLines);
        text = Replace(text, @"\s+", " ");
        foreach (var item in Blacklist)
        {
            text = text.Replace(item, "");
        }
        return text;
    }

	/// <summary>
	/// Cleans up markdown text, outputting plaintext.
	/// </summary>
	/// <param name="markdownText"></param>
	/// <returns></returns>
    public static string MarkdownToPlainText(string markdownText)
    {
        string noHeaders = Replace(markdownText, @"(#+\s)", "");				 // Remove headers
		string noBoldItalics = Replace(noHeaders, @"(\*{1,2}|_{1,2})", "");   // Remove bold and italics
		string noLinks = Replace(noBoldItalics, @"\[(.*?)\]\(.*?\)", "$1");   // Replace links with link text only
		string noImages = Replace(noLinks, @"\!\[(.*?)\]\(.*?\)", "");		 // Remove images
		string noHtml = Replace(noImages, @"<[^>]*>", "");				     // Remove HTML tags
		string noLists = Replace(noHtml, @"^\s*([-*]|\d+\.)\s+",						 // Replace lists with plain text
			"", RegexOptions.Multiline);
		string plain = Replace(noLists, @"(```.*?```|`.*?`)", "");			 // Remove additional Markdown elements as necessary

		//Return plain text
		return plain;
    }


    /// <summary>
    /// Takes a block of HTML Text, like a rich RSS Summary
    /// and turns it into pretty plain ol' plaintext.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns>cleaned plaintext</returns>
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
        catch { return ""; }
    }
}
