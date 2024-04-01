using System.Globalization;
using System.Text.Json;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace HYDRANT;
public class Article
{
    #region DB
    public string Url { get; set; }
    public string Title { get; set; }
    public string RssSummary { get; set; }
    public DateTime PublishDate { get; set; }
    public bool Headline { get; set; }
    public bool Paywall { get; set; }
    public string Summary { get; set; }
    public string ImageURL { get; set; }
    public string Publisher { get; set; }
    public string Author { get; set; }

    public string PublisherIcon
    {
        get
        {
            string Favi = "https://" + new Uri(Url).Host + "/favicon.ico";
            return Favi;
        }
    }

    #endregion
    public string Content { get; set; }
    public bool JakeFilter { get; set; }

    private const string IP = "localhost";
    private const string User = "root";
    private const string Pass = "Mavik";
    private const string HallonEndpoint = "https://31.205.123.48:7255";
    private const bool ForceHallon = false;

    public static async Task<List<Article>> GetArticles(int Limit = 0, int Offset = 0)
    {
        if (ForceHallon) { return await GetArticlesFromHallon(Limit, Offset); }
#if WINDOWS10_0_18362_0_OR_GREATER || HAS_UNO_SKIA || __MACOS__ //Use raw DB Calls on supported platforms
        return GetArticlesFromDB(Limit, Offset);
#else //Other platforms, i.e WASM/IOS/Android etc. do not support code within MySQL so use HallonAPIServer for it
        return await GetArticlesFromHallon(Limit, Offset);
#endif
    }

    /// <summary>
    /// This runs GetArticlesFromDB through HallonAPIServer, a middleware solution.
    /// </summary>
    /// <returns></returns>
    private static async Task<List<Article>?> GetArticlesFromHallon(int Limit = 0, int Offset = 0)
    {
        var endpoint = $"/Articles/GetArticles?limit={Limit}";


        //TODO: ACTUALLY IMPLEMENT SECURITY HERE
        //MITMs can occur here but since its read only DB call, nothing can really happen.
        //Nonetheless, this should be fixed when Hallon uses a proper certificate
        using (var client = new HttpClient(new CertificateIgnorer()))
        {
            try
            {
                var response = await client.GetAsync(HallonEndpoint + endpoint);
                response.EnsureSuccessStatusCode(); // Throw an exception if not successful

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Article>>(content);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request exception: {e.Message}");
                return new();
            }
            catch (Exception e)
            {
                Console.WriteLine($"General exception: {e.Message}");
                return new();
            }
        }
    }

    public static List<Article> GetArticlesFromDB(int Limit = 0, int Offset = 0, string ConnectionString = $"server={IP};user={User};database=fhdb;port=3306;password={Pass}")
    {
        string query = $@"
SELECT URL, TITLE, RSS_SUMMARY, PUBLISH_DATE, HEADLINE, PAYWALL, SUMMARY, ImageURL, ARTICLE_TEXT, PUBLISHER, AUTHOR 
FROM ARTICLES 
ORDER BY PUBLISH_DATE DESC
LIMIT {Limit} OFFSET {Offset};
";

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
                Headline = Convert.ToBoolean(reader["HEADLINE"]),
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
                (TITLE, URL, ARTICLE_TEXT, PUBLISH_DATE, RSS_SUMMARY, SUMMARY, WEIGHTING, HEADLINE, BUSINESS_RELATED, PAYWALL, COMPANIES_MENTIONED, EXECUTIVES_MENTIONED, JAKE_FLAG, ImageURL, PUBLISHER, AUTHOR) 
                VALUES 
                (@Title, @Url, @Content, @PublishDate, @RssSummary, @Summary, @Weighting, @Headline, @BusinessRelated, @Paywall, @CompaniesMentioned, @ExecutivesMentioned, @JakeFlag, @ImageURL, @PUBLISHER, @AUTHOR)";
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@Url", Url);
                    command.Parameters.AddWithValue("@Content", Content);
                    command.Parameters.AddWithValue("@PublishDate", PublishDate);
                    command.Parameters.AddWithValue("@RssSummary", RssSummary);
                    command.Parameters.AddWithValue("@Summary", Summary);
                    command.Parameters.AddWithValue("@Weighting", 0);
                    command.Parameters.AddWithValue("@Headline", Headline);
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
}
