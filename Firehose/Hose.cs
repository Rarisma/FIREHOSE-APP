using System.Collections.Concurrent;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Xml;
using HYDRANT;
//Fortune and glory, kid.
namespace Firehose;
/// <summary>
/// RSS feeds go in.
/// News analysis comes out.
/// </summary>
public static class Hose
{
    private static HashSet<String> RSSFeeds = new();
    private static HashSet<Article> Articles = new();
    private static HashSet<String> URLs = new();
    private static List<String> RestrictedURLs = new() { "/kals-cartoon", "www.economist.com", "https://www.nytimes.com/video/", "/images/" };
    private static List<String> RestrictedStrings  = new() { "Please enable JS and disable any ad blocker",
        "Can't honor your request at this time.", "",
        "Complete digital access to quality FT journalism with expert analysis from industry leaders. Pay a year upfront and save 20%." };
    private static int totalFeeds;
    private static int processedFeeds;
    private static int errors;
    private static int dupes;
    static Dictionary<string, SyndicationItem> Stories = new();

    public static void Init()
    {
        ReloadSources();
        GetStories();
    }

    static async void GetStories()
    {
        Stopwatch sw = Stopwatch.StartNew();
        totalFeeds = RSSFeeds.Count; // Assuming RSSFeeds is a List or similar collection
        ConcurrentDictionary <string, SyndicationItem> Unfiltered = new();
        Parallel.ForEach(RSSFeeds, new(){MaxDegreeOfParallelism = 8}, URL =>
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        using (XmlReader reader = XmlReader.Create(URL, new() { DtdProcessing = DtdProcessing.Parse }))
                        {
                            SyndicationFeed feed = SyndicationFeed.Load(reader);
                            foreach (SyndicationItem item in feed.Items)
                            {
                                try
                                {
                                    Unfiltered.TryAdd(item.Id, item); // Thread-safe add
                                }
                                catch (ArgumentException)
                                {
                                    Interlocked.Increment(ref dupes); // Thread-safe increment
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Interlocked.Increment(ref errors); // Thread-safe increment for errors
                    }
                }, cts.Token);

                try
                {
                    task.Wait(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // The task took longer than 10 seconds to complete, handle accordingly
                }
            }

            Interlocked.Increment(ref processedFeeds); // Thread-safe increment
                                                       // UpdateProgressBar might need to be invoked on the UI thread if this is a GUI application
                                                       // Consider using BeginInvoke or Invoke for WinForms/WPF/WinUI3 applications
        });
        sw.Stop();
        Console.WriteLine($"Story collection completed in {sw.Elapsed.Seconds} Seconds.");

        sw.Restart();
        //Date filter
        Console.WriteLine($"RSS Feeds Contain {Unfiltered.Count} stories\nFiltering by date...");
        try
        {
            Stories = Unfiltered.Where(kvp => kvp.Value.PublishDate > DateTimeOffset.Now.AddDays(-2))
                .ToDictionary(item => item.Key, item => item.Value);

        }
        catch
        {
            Console.WriteLine("Error occured filtering by date");
        }

        try
        {
            //Blacklist filter
            Console.WriteLine($"RSS Feeds Contain {Stories.Count} stories\nFiltering by blacklist...");
            List<string> bl = Article.GetURLs();
            Stories = Stories.Where(pair => !bl.Contains(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        catch
        {
            Console.WriteLine("Error occured filtering by blacklist.");
        }


        // Filter by restricted URLs
        Console.WriteLine($"RSS Feeds Contain {Stories.Count} stories\nFiltering by Restricted URLs...");
        Stories = Stories.Where(pair => !RestrictedURLs.Any(sub => pair.Key.Contains(sub))).ToDictionary(pair => pair.Key, pair => pair.Value);
        Console.WriteLine($"RSS Feeds Contain {Stories.Count} stories.\nGathering data and filtering out blocked articles. (THIS WILL TAKE A LONG TIME)\n");



        // Get website data and remove blocked ones.
        int error = 0;
        ConcurrentBag<Article> tempart = new();
        Parallel.ForEach(Stories.Values, async (Article) =>
        {

        });
        foreach (var Article in Stories.Values)
        {
            try
            {
                Article newArticle = new()
                {
                    Url = Article.Links.FirstOrDefault()?.Uri?.ToString(),
                    Title = Article.Title.Text,
                    RssSummary = Article.Summary?.Text,
                    Content = await SubjugationService.GetURLText(Article.Links.FirstOrDefault()?.Uri?.ToString()),
                    PublishDate = Article.PublishDate.DateTime,
                    LowQuality = false,
                    Paywall = false,
                    Summary = ""
                };
                tempart.Add(newArticle);
                UpdateProgressBar(tempart.Count, Stories.Count, $" {error} errors");
            }
            catch
            {
                Interlocked.Increment(ref error);
            }
        }
        Articles = tempart.Where(article => !RestrictedStrings.Contains(article.Content)).ToHashSet();
        Console.WriteLine($"RSS Feeds Contain {Stories.Count} stories.");

        sw.Stop();
        Console.WriteLine($"Filtering completed in {sw.Elapsed.Seconds} Seconds.");

        Console.WriteLine($"Summarising stories, this will take fucking ages.\n");
        sw.Restart();
        foreach (var article in Articles)
        {
            article.Summary = await LLM.OneShot(new()
            {
                new()
                {
                    content = """
                              Your task is to craft concise summaries for provided stories, each limited to a single paragraph.
                              Start each summary with the word 'Summary:' followed by the key points and narrative essence of the story.
                              Ensure you exclude any headings, titles, or introductory labels other than 'Summary:'.
                              The objective is to succinctly capture the core details and relevant content of each story.
                             
                              Your output MUST be no more than a paragraph.
                              Your output MUST just be the summary.
                              Your output MUST NOT start with summary: or any other introduction to the summary.
                              """,
                    role = "system"
                },
                new() { content = $"Summarise the following {article.Content}", role = "user" },
            });
            article.Upload();
            UpdateProgressBar(Articles.ToList().IndexOf(article), Articles.Count, "");
        } 
        
        Console.WriteLine($"Story analysis complete in {sw.Elapsed.Minutes} minutes.");
    }

    static void UpdateProgressBar(int completed, int total, string exdata = null)
    {
        if (exdata == null) { exdata = $"(Errors: {errors}, Stories: {Stories.Count}, Dupes: {dupes})"; }
        
        Console.CursorLeft = 0; // Reset cursor position to the beginning of the current line

        // Clear the current line by writing spaces
        Console.Write(new string(' ', Console.WindowWidth));

        // Reset cursor position again after clearing the line
        Console.CursorTop--;
        Console.CursorLeft = 0;

        int barWidth = 50; // Width of the progress bar in characters
        int progress = (int)((completed / (double)total) * barWidth);

        // Ensure the output fits within the console window width to avoid automatic line breaks
        string output = $"[{new string('=', progress)}{new string(' ', barWidth - progress)}] {completed}/{total} completed  {exdata}";

        Console.WriteLine(output);

        if (completed == total) Console.WriteLine();
    }
    
    /// <summary>
    /// Reloads all sources from RSSFeed.txt
    /// </summary>
    static void ReloadSources()
    {
        Console.WriteLine("Reloading Feeds.");
        RSSFeeds.Clear(); //Clear feeds
        foreach (var Line in File.ReadAllLines("RSSFeeds.txt"))
        {
            //Skip Commented lines
            if (Line[0] == '#') {continue;}

            //Add feed
            RSSFeeds.Add(Line);
        }
        Console.WriteLine($"Loaded {RSSFeeds.Count} feeds");
    }
}