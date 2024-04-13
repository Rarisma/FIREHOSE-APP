using System.ServiceModel.Syndication;
using System.Xml;
using HYDRANT;
//Battle for everyone's source code.
namespace Firehose2;
internal class ArticleDiscovery
{
    /// <summary>
    /// Feeds ready to be processed by the next stage.
    /// </summary>
    public static Dictionary<string, SyndicationItem> Pending = new();
    public static Dictionary<string, SyndicationItem> PendingLowPriority = new();
    private static List<string> BlockedURLS = new()
    {
        "/kals-cartoon", "www.economist.com", "https://www.nytimes.com/video/", "/images/"
    };
    private static List<string> URLBlacklist = new();

    public static void StartFilter()
    {
        List<string> Feeds = GetFeeds();

        while (true)
        {
            RefreshBlacklist();
            foreach (var URL in Feeds)
            {
                try
                {

                    using (new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                    {
                        try
                        {
                            using (XmlReader Reader =
                                   XmlReader.Create(URL, new() { DtdProcessing = DtdProcessing.Parse }))
                            {
                                SyndicationFeed feed = SyndicationFeed.Load(Reader);
                                foreach (SyndicationItem item in feed.Items)
                                {

                                    try
                                    {
                                        //Check we haven't already scanned this article
                                        if (URLBlacklist.Contains(item.Links.FirstOrDefault()?.Uri?.ToString()) 
                                            | BlockedURLS.Any(item.Id.Contains))
                                        {
                                            continue;
                                        }

                                        if ((DateTime.Now - item.PublishDate).TotalDays <= 5)
                                        {
                                            Pending.Add(item.Links.FirstOrDefault()?.Uri?.ToString(), item);
                                        }

                                        URLBlacklist.Add(item.Links.FirstOrDefault()?.Uri?.ToString());
                                    }
                                    catch
                                    {
                                        continue;
                                    }

                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // The task took longer than 10 seconds to complete, skip the article.
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    continue;

                }
            }

            Console.WriteLine("Article Discovery Iteration Complete.");
            while (Pending.Count >= 10 && PendingLowPriority.Count >= 10)
            {
                Thread.Sleep(900000);
            }
        }
    }



    static void RefreshBlacklist()
    {
        Console.WriteLine("Refreshing URL Blacklist");
        URLBlacklist.AddRange(Article.GetURLs());
        URLBlacklist = URLBlacklist.Distinct().ToList();
        Console.WriteLine($"URL Blacklist has {URLBlacklist.Count}\n");
    }

    /// <summary>
    /// Loads feeds from within /Data/RSSFeeds
    /// </summary>
    /// <returns></returns>
    private static List<string> GetFeeds()
    {
        Console.WriteLine("Loading Feeds.");
        List<string> Feeds = new();
        foreach (var Line in File.ReadAllLines("Data/RSSFeeds.txt"))
        {
            //Skip Commented lines
            if (Line[0] == '#') { continue; }

            //Add feed
            Feeds.Add(Line);
        }

        Console.WriteLine($"Loaded {Feeds.Count} feeds");
        return Feeds;
    }
}
