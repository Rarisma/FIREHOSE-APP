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


public class Program
{

    static void Main(string[] args)
    {




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
