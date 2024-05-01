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
    public static Dictionary<string, (SyndicationItem item, int PublicationID)> Pending = new();
    private static List<string> BlockedURLS = new()
    {
        "/kals-cartoon", "www.economist.com", "https://www.nytimes.com/video/", "/images/"
    };
    private static List<string> URLBlacklist = new();

    public static void StartFilter()
    {
        List<Publication> Feeds = Publication.LoadFromJSON();

        while (true)
        {
            RefreshBlacklist();
            foreach (var Feed in Program.Publications)
            {
                foreach (var URL in Feed.URLs)
                {
                    ProcessFeed(URL, Feed.ID);
                }

            }

            Console.WriteLine("Article Discovery Iteration Complete.");
            while (Pending.Count >= 10)
            {
                Thread.Sleep(900000);
            }
        }
    }

    private static void ProcessFeed(string URL, int PublicationID)
    {
        //Skip example publication
        if (URL.Contains("rarisma.net")) { return;}

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
                                    Pending.Add(item.Links.FirstOrDefault()?.Uri?.ToString(), (item, PublicationID));
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
                catch
                {

                }
            }
        }
        catch (OperationCanceledException)
        {
            // The task took longer than 10 seconds to complete, skip the feed.
        }
    }



    static void RefreshBlacklist()
    {
        Console.WriteLine("Refreshing URL Blacklist");
        URLBlacklist.AddRange(Article.GetURLs());
        URLBlacklist = URLBlacklist.Distinct().ToList();
        Console.WriteLine($"URL Blacklist has {URLBlacklist.Count}\n");
    }
}
