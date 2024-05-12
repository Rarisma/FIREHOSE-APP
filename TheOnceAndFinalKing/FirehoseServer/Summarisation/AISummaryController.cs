using FirehoseServer.Stocks;

namespace FirehoseServer.Summarisation;

/// <summary>
/// Controls summarization stuff.
/// </summary>
public static class AISummaryController
{
	public static void Start()
	{
		Console.WriteLine("API Server Started, press enter to start summarising stories");
		Console.ReadLine();

		CompanyData.LoadTickers(); //Load company data.

		//Start discovering RSS feeds.
		Thread ArticleDiscoveryThread = new(ArticleDiscovery.StartFilter);
		ArticleDiscoveryThread.Start();

		//Scrape URLs
		Thread ArticleScrapingThread = new(ArticleScraper.StartScraping);
		ArticleScrapingThread.Start();

		//Load LLM feeds.
		for (int id = 0; id < LLM.Endpoints.Length; id++)
		{
			Thread ArticleAnalyserThread = new(new ArticleAnalyser(id).StartAnalysis);
			ArticleAnalyserThread.Start();
		}

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
}