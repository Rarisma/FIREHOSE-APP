using System.Diagnostics;
using FirehoseServer.Stocks;
using HYDRANT.Definitions;

namespace FirehoseServer.Summarisation;

/// <summary>
/// Controls summarization stuff.
/// </summary>
public static class AISummaryController
{
	public static async void Start()
	{
		Console.WriteLine("API Server Started, press enter to start summarising stories (Type U for update)");

		await CompanyData.LoadTickers(); //Load company data.
        Console.WriteLine($"Loaded {CompanyData.Tickers.Count} tickers.");
        var x = CompanyData.Tickers.Where(x => string.IsNullOrWhiteSpace(x.Ticker));
        //Sanity check tickers.
        Console.WriteLine($"Found {CompanyData.Tickers.Count(x => string.IsNullOrWhiteSpace(x.Name))} tickers without a name");
        Console.WriteLine($"Found {CompanyData.Tickers.Count(x => string.IsNullOrWhiteSpace(x.Ticker))} names without a ticker");

        if (Console.ReadLine() == "U")  //Go to update mode if picked
        {
            UpdateStories();
            return;
        }
        //Start discovering RSS feeds.
        Thread ArticleDiscoveryThread = new(ArticleDiscovery.StartFilter);
		ArticleDiscoveryThread.Start();

		//Scrape URLs
		Thread ArticleScrapingThread = new(ArticleScraper.StartScraping);
		ArticleScrapingThread.Start();

        Thread ArticleAnalyserThread = new(new ArticleAnalyser(0).StartAnalysis);
        ArticleAnalyserThread.Start();


		//Loop print 'dashboard'
		while (true)
		{
			Console.Write(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(0, Console.CursorTop);

			Console.WriteLine($"| Discovered:{ArticleDiscovery.Pending.Count} " +
			                  $"| Scraped: {ArticleScraper.ProcessedArticles.Count} " +
			                  $"| Total Analysed: {ArticleAnalyser.TotalScraped} |");
			Console.CursorTop--;
			Console.CursorLeft = 0;
			Thread.Sleep(200);
		}
	}
    
    
    public static async void UpdateStories()
    {
        Console.WriteLine("WARNING, YOU ARE NOW IN UPDATE MODE NO GUARDRAILS WILL APPLY!");
        Console.WriteLine("Type an SQL Query to find articles to update.");
        string SQLQuery = Console.ReadLine();
        List<Article> articles =  Program.MySQLReadOnly.GetArticles(999999999, 0, SQLQuery);
        
        if (articles.Count >= 10000)
        {
            Console.WriteLine("WHOA THERE POINDEXTER, ARE YOU TRYING UPDATE 10K+ ARTICLES?");
        }

        Console.WriteLine($"""
                          Found {articles.Count} articles, to be updated.
                          Select field to update.
                          
                          1) Headline
                          2) Summary
                          3) Article Text & Authors
                          4) Companies Mentioned
                          5) Sectors
                          
                          Tip: doing this is incredibly time expensive, consider updating multiple fields at once.
                          You can specify multiple rows with commas, i.e. 1,2,4,5 
                          """);
        
        string Options = Console.ReadLine();
        Stopwatch s = Stopwatch.StartNew();
        foreach (Article art in articles)
        {
            // Re obtain article text first if needed.
            if (Options.Contains("3"))
            {
                var res = await StringTools.GetURLText(art.Url, art.IsPaywall);
                art.Text = res.Item1;
                art.Author = res.Item2;
            }   

            if (Options.Contains("1")) { art.IsHeadline = await ArticleAnalyser.Headline(art.Text, 0); }   // Redetermine headline       
            if (Options.Contains("2")) { art.Summary = await ArticleAnalyser.Summarise(art.Text, 0); }  // Re summarise article
            if (Options.Contains("4")) { art.CompaniesMentioned = Company.FindCompaniesInText(art.Text, CompanyData.Tickers); }  // Recheck article for companies
            if (Options.Contains("5")) { art.Sectors = await ArticleAnalyser.ClassifyArticle(art.Text, 0); }  // Recheck article for companies

            //Update article in db
            Program.MySQLAdmin.UpdateArticle(art);
        }
        
        Console.WriteLine($"Finished updating in {s.Elapsed.TotalMinutes} minutes");
    }
}
