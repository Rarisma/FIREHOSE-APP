using System.Globalization;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;

namespace HYDRANT;
public class Article
{
    #region DB
    public string Url { get; set; }
    public string Title { get; set; }
    public string RssSummary { get; set; }
    public DateTime PublishDate { get; set; }
    public bool LowQuality { get; set; }
    public bool Paywall { get; set; }
    public string Summary { get; set; }
    public string ImageURL { get; set; }
    public string Publisher { get; set; }
    public string Author { get; set; }

    public string PublisherIcon
    {
        get
        {
            string Favi = "http://" + new Uri(Url).Host + "/favicon.ico";
            return Favi;
        }
    }

    public string TimeFromPublication
    {
        get
        {
            return Math.Round((DateTime.Now - PublishDate).TotalHours) + "hrs ago";
        }
    }
    #endregion
    public string Content { get; set; }
    public bool JakeFilter { get; set; }

    private const string IP = "localhost";
    private const string User = "root";
    private const string Pass = "Mavik";
    public static List<Article> GetArticles(int Limit = 0, string ConnectionString = $"server={IP};user={User};database=fhdb;port=3306;password={Pass}")
    {
        string query = $"SELECT URL, TITLE, RSS_SUMMARY, PUBLISH_DATE, LOW_QUALITY, PAYWALL, SUMMARY, ImageURL, ARTICLE_TEXT, PUBLISHER, AUTHOR FROM ARTICLES LIMIT {Limit} OFFSET 200";

        //Connection to the DB
        MySqlConnection DB = new();
        DB.ConnectionString = ConnectionString;
        DB.Open();

        MySqlCommand cmd = new MySqlCommand(query, DB);
        MySqlDataReader reader = cmd.ExecuteReader();
        List<Article> articles = new();
        while (reader.Read())
        {
            var article = new Article
            {
                Url = reader["URL"].ToString(),
                Title = reader["TITLE"].ToString(),
                RssSummary = reader["RSS_SUMMARY"].ToString(),
                PublishDate = Convert.ToDateTime(reader["PUBLISH_DATE"]),
                LowQuality = Convert.ToBoolean(reader["LOW_QUALITY"]),
                Paywall = Convert.ToBoolean(reader["PAYWALL"]),
                Summary = reader["SUMMARY"].ToString(),
                ImageURL = reader["ImageURL"].ToString(),
                Content = reader["ARTICLE_TEXT"].ToString(),
                Publisher = reader["PUBLISHER"].ToString(),
                Author = reader["AUTHOR"].ToString(),
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                INSERT INTO ARTICLES 
                (TITLE, URL, ARTICLE_TEXT, PUBLISH_DATE, RSS_SUMMARY, SUMMARY, WEIGHTING, LOW_QUALITY, BUSINESS_RELATED, PAYWALL, COMPANIES_MENTIONED, EXECUTIVES_MENTIONED, JAKE_FLAG, ImageURL, PUBLISHER, AUTHOR) 
                VALUES 
                (@Title, @Url, @Content, @PublishDate, @RssSummary, @Summary, @Weighting, @LowQuality, @BusinessRelated, @Paywall, @CompaniesMentioned, @ExecutivesMentioned, @JakeFlag, @ImageURL, @PUBLISHER, @AUTHOR)";
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@Url", Url);
                    command.Parameters.AddWithValue("@Content", Content);
                    command.Parameters.AddWithValue("@PublishDate", PublishDate);
                    command.Parameters.AddWithValue("@RssSummary", RssSummary);
                    command.Parameters.AddWithValue("@Summary", Summary);
                    command.Parameters.AddWithValue("@Weighting", 0);
                    command.Parameters.AddWithValue("@LowQuality", LowQuality);
                    command.Parameters.AddWithValue("@BusinessRelated", false);
                    command.Parameters.AddWithValue("@Paywall", false);
                    command.Parameters.AddWithValue("@CompaniesMentioned", "");
                    command.Parameters.AddWithValue("@ExecutivesMentioned", "");
                    command.Parameters.AddWithValue("@JakeFlag", false);
                    command.Parameters.AddWithValue("@ImageURL", ImageURL);
                    command.Parameters.AddWithValue("@PUBLISHER", Publisher);
                    command.Parameters.AddWithValue("@AUTHOR", Author);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }


    /// <summary>
    /// Gets the image for a URL from the OpenGraph Metadata
    /// </summary>
    /// <param name="URL">URL to scrape</param>
    /// <returns>OpenGraph Image URL</returns>
    public static async Task<string> GetImageForURL(string URL)
    {
        using (var httpClient = new HttpClient())
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(await httpClient.GetStringAsync(URL));

            var ogImageNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
            if (ogImageNode != null)
            {
                return ogImageNode.GetAttributeValue("content", string.Empty);
            }
            return "?";
        }
    }

}
