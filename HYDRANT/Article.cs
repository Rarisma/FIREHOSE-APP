using System.Runtime.InteropServices;
using System.Text.Json;
using MySql.Data.MySqlClient;

namespace HYDRANT;
public class Article
{
    #region DB
    /// <summary>
    /// Title of article
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// URL of article
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Article Text
    /// </summary>
    public string ArticleText { get; set; }

    /// <summary>
    /// The given RSS Summary via the publication
    /// These aren't used currently and probably never will
    /// but they could be useful for analysis in the future
    /// </summary>
    public string RSSSummary { get; set; }

    /// <summary>
    /// Date/Time the article was made public
    /// </summary>
    public DateTime PublishDate { get; set; }

    /// <summary>
    /// Is this a major story, i.e. something major is going on
    /// or is the article not a major event
    /// </summary>
    public bool Headline { get; set; }

    /// <summary>
    /// is the article from a paywalled source
    /// </summary>
    public bool Paywall { get; set; }

    /// <summary>
    /// AI-Generated summary of the article
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Graph-QL image
    /// </summary>
    public string ImageURL { get; set; }
    /// <summary>
    /// Firehose Publication ID
    /// </summary>
    public int PublisherID { get; set; }

    /// <summary>
    /// Does the article relate to business?
    /// </summary>
    public bool Buisness { get; set; }

    /// <summary>
    /// Company names/tickers mentioned within the article.
    /// </summary>
    public string CompaniesMentioned { get; set; }

    #endregion

    /// <summary>
    /// Internally used
    /// </summary>
    public bool JakeFilter { get; set; }

    private const string IP = "localhost";
    private const string User = "root";
    private const string Pass = "Mavik";
    public const string HallonEndpoint = "https://www.hallon.rarisma.net:5000";


    /// <summary>
    /// This runs GetArticlesFromDB through Hallon API Server, a middleware solution.
    /// </summary>
    /// <returns>List of articles</returns>
    public static async Task<List<Article>> GetArticlesFromHallon(int Limit = 0,
        int Offset = 0, string Filter = "ORDER BY PUBLISH_DATE DESC")
    {
        var endpoint = $"/Articles/GetArticles?limit={Limit}&offset={Offset}&filter={Filter}";

        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(HallonEndpoint + endpoint);
                response.EnsureSuccessStatusCode(); // Throw an exception if not successful

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Article>>(content)!;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request exception: {e.Message}");
                return new()
                {
                    new()
                    {
                        Title = "Failed to load articles!",
                        RSSSummary = e.Message,
                        PublisherID = 0,
                        ImageURL = "?",
                        PublishDate = new DateTime(1911, 11,19),
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"General exception: {e.Message}");
                return new()
                {
                    new()
                    {
                        Title = "Failed to load articles!",
                        RSSSummary = e.Message,
                        PublisherID = 0,
                        ImageURL = "?",
                        PublishDate = new DateTime(1911, 11,19),
                    }
                };
            }
        }
    }

    public static List<Article> GetArticlesFromDB(int Limit = 0, int Offset = 0,
        string ConnectionString = $"server={IP};user={User};database=fhdb;port=3306;password={Pass}",
        string Filter = "ORDER BY PUBLISH_DATE DESC")
    {
        string query = $@"SELECT URL, TITLE, RSS_SUMMARY, PUBLISH_DATE, HEADLINE, PAYWALL, SUMMARY, ImageURL,
        ARTICLE_TEXT, PUBLISHER_ID FROM ARTICLES {Filter} LIMIT {Limit} OFFSET {Offset};";

        //Connection to the DB
        MySqlConnection DB = new() {ConnectionString = ConnectionString};
        DB.Open();

        MySqlCommand cmd = new MySqlCommand(query, DB);
        MySqlDataReader reader = cmd.ExecuteReader();
        List<Article> articles = new();
        while (reader.Read())
        {
            var article = new Article
            {
                Title = reader["TITLE"].ToString(),
                Url = reader["URL"].ToString(),
                RSSSummary = reader["RSS_SUMMARY"].ToString(),
                PublishDate = Convert.ToDateTime(reader["PUBLISH_DATE"]),
                Headline = Convert.ToBoolean(reader["HEADLINE"]),
                Paywall = Convert.ToBoolean(reader["PAYWALL"]),
                Summary = reader["SUMMARY"].ToString(),
                ImageURL = reader["ImageURL"].ToString(),
                ArticleText = reader["ARTICLE_TEXT"].ToString(),
                PublisherID = (int)reader["PUBLISHER_ID"],
            };
            articles.Add(article);
        }
        reader.Close();
        DB.Close();
        return articles;
    }

    public static List<string> GetURLs(string ConnectionString = $"server={IP};user={User};database=fhdb;port=3306;password={Pass}")
    {
        //Connection to the DB
        MySqlConnection DB = new();
        DB.ConnectionString = ConnectionString;
        DB.Open();

        //execute command
        MySqlCommand cmd = new MySqlCommand("SELECT URL FROM ARTICLES", DB);
        MySqlDataReader reader = cmd.ExecuteReader();

        //get all URLs
        List<String> URLs = new();
        while (reader.Read()) { URLs.Add(reader["URL"].ToString()); }

        //Return URLs
        return URLs;
    }

    /// <summary>
    /// Uploads Article to DB
    /// </summary>
    /// <param name="ConnectionString"></param>
    public void Upload(string ConnectionString = $"server={IP};user={User};database=fhdb;port=3306;password={Pass}")
    {
        try
        {
            using (MySqlConnection connection = new(ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                INSERT INTO ARTICLES 
                (TITLE, URL, ARTICLE_TEXT, PUBLISH_DATE, RSS_SUMMARY, SUMMARY, WEIGHTING, HEADLINE, 
                BUSINESS_RELATED, PAYWALL, COMPANIES_MENTIONED, EXECUTIVES_MENTIONED, JAKE_FLAG,
                ImageURL, PUBLISHER_ID) 
                VALUES 
                (@Title, @Url, @Content, @PublishDate, @RssSummary, @Summary, @Weighting, @Headline, @BusinessRelated, @Paywall, @CompaniesMentioned, @ExecutivesMentioned, @JakeFlag, @ImageURL, @PUBLISHER_ID)";
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@Url", Url);
                    command.Parameters.AddWithValue("@Content", ArticleText);
                    command.Parameters.AddWithValue("@PublishDate", PublishDate);
                    command.Parameters.AddWithValue("@RssSummary", RSSSummary);
                    command.Parameters.AddWithValue("@Summary", Summary);
                    command.Parameters.AddWithValue("@Weighting", 0);
                    command.Parameters.AddWithValue("@Headline", Headline);
                    command.Parameters.AddWithValue("@BusinessRelated", Buisness);
                    command.Parameters.AddWithValue("@Paywall", Paywall);
                    command.Parameters.AddWithValue("@CompaniesMentioned", CompaniesMentioned);
                    command.Parameters.AddWithValue("@ExecutivesMentioned", "");
                    command.Parameters.AddWithValue("@JakeFlag", false);
                    command.Parameters.AddWithValue("@ImageURL", ImageURL);
                    command.Parameters.AddWithValue("@PUBLISHER_ID", PublisherID);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
