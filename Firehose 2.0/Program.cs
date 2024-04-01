//The "what's-it-called café"

using HYDRANT;

namespace Firehose2;
/* Firehose 2.0
 * Firehose is a News Data Aggregator, this is a complete rewrite.
 * Firehose's mission is to try and get every news article in real time.
 *
 * History of Firehose
 * Dec 2023 - Firehose 1.0
 * Under IAI, the concept of firehose was created.
 * Here it was written in python and the concept of firehose was proven.
 *
 * Jan/Fed 2024 Firehose 1.5
 * After IAI disappeared due to *reasons* Firehose was tweaked to
 * better serve news analysis instead of its original purpose.
 * It was also rewritten in C# with a few tweaks to make it run better,
 * this had major performance improvements compared to 1.0
 *
 * April 2024 Firehose 2.0
 * Firehose 2.0 is another rewrite of firehose to make the codebase more
 * professional and make further use of multi-threading to make sure we are
 * using 100% of the CPU at all times.
 *
 * Future Ideas:
 *  - Publisher information scraping
 *  - Author information scraping
 */
internal class Program
{
    static void Main(string[] args)
    {

        Console.WriteLine("Firehose Initialised");

        //Configure LLM Server
        Console.WriteLine("Checking for or loading LLM Server");
        LLM.LaunchServer();

        int IterationCount = 0;
        Console.WriteLine($"Starting Iteration {IterationCount}");
        Thread.Sleep(3256);
        LLM.SendPostRequest("you are a helpful assistant.", "hello, whats 2+2");

        Thread ArticleDiscoveryThread = new(ArticleDiscovery.StartFilter);
        ArticleDiscoveryThread.Start();

        Thread ArticleScrapingThread = new(ArticleScraper.StartScraping);
        ArticleScrapingThread.Start();

        Thread ArticleAnalyserThread = new(ArticleAnalyser.StartAnalysis);
        ArticleAnalyserThread.Start();

        while (true)
        {
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine($"Discovery Pending Priority:{ArticleDiscovery.Pending.Count}" +
                              $" non:{ArticleDiscovery.PendingLowPriority.Count} |" +
                              $" Processed: {ArticleScraper.ProcessedArticles.Count}");
            Console.CursorTop--;
            Console.CursorLeft = 0;
        }
    }
}
