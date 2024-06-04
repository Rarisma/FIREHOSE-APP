using HYDRANT.Definitions;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;
//The voice someone calls (in the labyrinth)
[assembly: InternalsVisibleTo("FirehoseServer")]
namespace Hydrant;
internal class SQL
{
	private string Connection;
	/// <summary>
	/// MySQL Connection String
	/// </summary>
	/// <param name="connectionString"></param>
	public SQL(string connectionString)
	{
		//Connect to the DB and open the connection
		Connection = connectionString;
	}


	/// <summary>
	/// Gets articles from database.
	/// </summary>
	/// <returns>list of articles.</returns>
	internal List<Article> GetArticles(int Limit = 0, int Offset = 0,
	string Filter = "ORDER BY PUBLISH_DATE DESC")
	{
		string query = $@"SELECT URL, TITLE, RSS_SUMMARY, PUBLISH_DATE, HEADLINE,
		PAYWALL, SUMMARY, ImageURL,ARTICLE_TEXT, PUBLISHER_ID, BUSINESS_RELATED,
		COMPANIES_MENTIONED, AUTHOR, SECTORS FROM ARTICLES {Filter} LIMIT {Limit}
		OFFSET {Offset};";

		//Run command
		MySqlConnection DB = new() { ConnectionString = Connection };
		DB.Open();

		MySqlCommand cmd = new MySqlCommand(query, DB);
		MySqlDataReader reader = cmd.ExecuteReader();
		List<Article> articles = new();
		while (reader.Read())
		{
			//'deserialize' into article
			var article = new Article
			{
				Url = reader["URL"].ToString() ?? string.Empty,
				Title = reader["TITLE"].ToString() ?? "",
				RSSSummary = reader["RSS_SUMMARY"].ToString() ?? "",
				PublishDate = Convert.ToDateTime(reader["PUBLISH_DATE"]),
				IsHeadline = Convert.ToBoolean(reader["HEADLINE"]),
				IsPaywall = Convert.ToBoolean(reader["PAYWALL"]),
				Summary = reader["SUMMARY"].ToString()!,
				ImageURL = reader["ImageURL"].ToString(),
				Text = reader["ARTICLE_TEXT"].ToString()!,
				PublisherID = (int)reader["PUBLISHER_ID"],
				Business = Convert.ToBoolean(reader["BUSINESS_RELATED"]),
				CompaniesMentioned = reader["COMPANIES_MENTIONED"].ToString()!,
				Author = reader["AUTHOR"].ToString()!,
				Sectors = reader["SECTORS"].ToString()!,
			};

			//Add to collection
			articles.Add(article);
		}
		
		//Dispose reader and return articles.
		DB.Close();
		reader.Close();
		return articles;
	}

	/// <summary>
	/// Get urls from database.
	/// </summary>
	/// <returns>List of URLs</returns>
	internal List<string> GetURLs()
	{
		//Create connection object
		MySqlConnection DB = new();
		DB.ConnectionString = Connection;
		DB.Open();

		//execute command
		MySqlCommand cmd = new("SELECT URL FROM ARTICLES", DB);
		MySqlDataReader reader = cmd.ExecuteReader();

		//Get all URLs, then 
		List<string> URLs = new();
		while (reader.Read()) { URLs.Add(reader["URL"].ToString() ?? string.Empty); }
		DB.Close();
		return URLs; //Return URL List
	}

	/// <summary>
	/// Uploads Article to DB
	/// </summary>
	/// <param name="Article">Article to post</param>
	internal void UploadArticle(Article Article)
	{
		try
		{
			using (MySqlConnection connection = new(Connection))
			{
				connection.Open();
				using (MySqlCommand command = new())
				{
					command.Connection = connection;
					command.CommandText = @"
                INSERT INTO ARTICLES 
                (TITLE, URL, ARTICLE_TEXT, PUBLISH_DATE, RSS_SUMMARY, SUMMARY, WEIGHTING,
				HEADLINE, BUSINESS_RELATED, PAYWALL, COMPANIES_MENTIONED, 
				EXECUTIVES_MENTIONED, ImageURL, PUBLISHER_ID, AUTHOR, SECTORS) 
                VALUES 
                (@Title, @Url, @Content, @PublishDate, @RssSummary, @Summary, @Weighting,
				@Headline, @BusinessRelated, @Paywall, @CompaniesMentioned, 
				@ExecutivesMentioned, @ImageURL, @PUBLISHER_ID, @AUTHOR, @SECTORS)";
					command.Parameters.AddWithValue("@Title", Article.Title);
					command.Parameters.AddWithValue("@Url", Article.Url);
					command.Parameters.AddWithValue("@Content", Article.Text);
					command.Parameters.AddWithValue("@PublishDate", Article.PublishDate);
					command.Parameters.AddWithValue("@RssSummary", Article.RSSSummary);
					command.Parameters.AddWithValue("@Summary", Article.Summary);
					command.Parameters.AddWithValue("@Weighting", 0);
					command.Parameters.AddWithValue("@Headline", Article.IsHeadline);
					command.Parameters.AddWithValue("@BusinessRelated", Article.Business);
					command.Parameters.AddWithValue("@Paywall", Article.IsPaywall);
					command.Parameters.AddWithValue("@CompaniesMentioned", Article.CompaniesMentioned);
					command.Parameters.AddWithValue("@ExecutivesMentioned", "");
					command.Parameters.AddWithValue("@ImageURL", Article.ImageURL);
					command.Parameters.AddWithValue("@PUBLISHER_ID", Article.PublisherID);
					command.Parameters.AddWithValue("@AUTHOR", Article.Author);
					command.Parameters.AddWithValue("@SECTORS", Article.Sectors);
					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

    /// <summary>
    /// Updates an existing article in the DB.
    /// </summary>
    /// <param name="article">Article to update</param>
    internal void UpdateArticle(Article article)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(Connection))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                UPDATE ARTICLES SET
                TITLE = @Title, 
                URL = @Url, 
                ARTICLE_TEXT = @Content, 
                PUBLISH_DATE = @PublishDate, 
                RSS_SUMMARY = @RssSummary, 
                SUMMARY = @Summary, 
                WEIGHTING = @Weighting,
                HEADLINE = @Headline, 
                BUSINESS_RELATED = @BusinessRelated, 
                PAYWALL = @Paywall, 
                COMPANIES_MENTIONED = @CompaniesMentioned, 
                EXECUTIVES_MENTIONED = @ExecutivesMentioned, 
                ImageURL = @ImageURL, 
                PUBLISHER_ID = @PUBLISHER_ID, 
                AUTHOR = @AUTHOR, 
                SECTORS = @SECTORS 
                WHERE URL = @Url";

                    command.Parameters.AddWithValue("@Title", article.Title);
                    command.Parameters.AddWithValue("@Url", article.Url);
                    command.Parameters.AddWithValue("@Content", article.Text);
                    command.Parameters.AddWithValue("@PublishDate", article.PublishDate);
                    command.Parameters.AddWithValue("@RssSummary", article.RSSSummary);
                    command.Parameters.AddWithValue("@Summary", article.Summary);
                    command.Parameters.AddWithValue("@Weighting", article.Weighting);
                    command.Parameters.AddWithValue("@Headline", article.IsHeadline);
                    command.Parameters.AddWithValue("@BusinessRelated", article.Business);
                    command.Parameters.AddWithValue("@Paywall", article.IsPaywall);
                    command.Parameters.AddWithValue("@CompaniesMentioned", article.CompaniesMentioned);
                    command.Parameters.AddWithValue("@ExecutivesMentioned", article.ExecsMentioned);
                    command.Parameters.AddWithValue("@ImageURL", article.ImageURL);
                    command.Parameters.AddWithValue("@PUBLISHER_ID", article.PublisherID);
                    command.Parameters.AddWithValue("@AUTHOR", article.Author);
                    command.Parameters.AddWithValue("@SECTORS", article.Sectors);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    public void IncrementClickbaitCounter(string article)
    {
        using (var connection = new MySqlConnection(Connection))
        {
            connection.Open();
            // Create the SQL command to increment the value for the specific row
            string sql = "UPDATE ARTICLES SET TimesReportedAsClickbait = TimesReportedAsClickbait + 1 WHERE URL = @id";
            
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", article);
                
                // Execute the command
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new Exception("No rows were updated for a clickbait report.");
                }
            }
        }
    }
}
