//The "what's-it-called café"
using Firehose2.Stocks;

namespace Firehose2;
/* WARNING
 * FIREHOSE IS PROVIDED COMPLETELY AS IS.
 * No support on getting firehose running or working correctly
 * will be provided as it is consistently in flux. Meaning any part
 * of it bar the API server can change at any time; I aim for firehose
 * to work on any sufficiently powerful machine, but I do not
 * guarantee it as I will frequently make tweaks for it to run as
 * optimally on my machine as possible
 *
 * So for firehose to work as intended you need system similar to mine
 *
 * Specs
 * - Windows 11
 * - MySQL with Firehose.db ran
 * - LocalHost user set up
 * - CPU: AMD 5800x
 * - GPU1: NVIDIA RTX 3090
 * - GPU2 (unused): GTX 1060
 * - CUDA Toolkit
 * - LM Studio running 2x
 *   Gemma 1.1 IT LLM with GemmaOptimized Preset
 *      - 300 Token Cutoff
 *      - 0.2 temp
 *      - 10 top-k
 *      - Max GPU layers
 *
 * PENDING CHANGES:
 *  Pending changes are changes I plan to make, these may or may not happen.
 *  - Move MySQL to Raspberry Pi or other offsite database
 *  - Move MySQL to use anonymous user
 *  - Ditch LM Studio for LlamaSharp
 *  - Test GTX 1060 for speeding up firehose
 *  - Automated scraping of stock markets
 *
 */

/* Firehose 2.0
 * Firehose is a News Data Aggregator, this is a complete rewrite from python.
 * Firehose's mission is to try and get every news article in real time.
 *
 * History of Firehose
 * Dec 2023 - Firehose 1.0
 * Under IAI, the concept of firehose was created.
 * Here it was written in python and the concept of firehose was proven.
 *
 * Jan/Feb 2024 Firehose 1.5
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
 *
 * May 2024 Firehose 3.0
 * Firehose 3.0 is a massive overhaul on how Firehose works:
 *  - Firehose now uses publication IDs to track publishers
 *  - HallonAPIServer is now merged into Firehose as a server thread
 *  - Add multiple endpoints for parsing stories at the same time
 *  - Switch to Gemma 1.1 it
 *
 * May 2024 -  BELLWETHER/3.1
 * Start to locate all references to companies.
 * Undo Hallon API Server (Raspberrys are now free again)
 *  
 */
public class Program
{

    static void Main(string[] args)
    {
        Console.WriteLine("Firehose Initialised");

        Console.WriteLine("API Server Started, press enter to start summarising stories");
        Console.ReadLine();

        CompanyData.LoadTickers();
        //SyncFeeds();

        //Configure LLM Server
        Console.WriteLine("Checking for or loading LLM Server");
        //LLM.LaunchServer();
        Thread.Sleep(8000);

        Thread ArticleDiscoveryThread = new(ArticleDiscovery.StartFilter);
        ArticleDiscoveryThread.Start();

        Thread ArticleScrapingThread = new(ArticleScraper.StartScraping);
        ArticleScrapingThread.Start();


        for (int id = 0; id < LLM.Endpoints.Length; id++)
        {
            Thread ArticleAnalyserThread = new(new ArticleAnalyser(id).StartAnalysis);
            ArticleAnalyserThread.Start();
        }


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
