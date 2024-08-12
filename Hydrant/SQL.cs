using HYDRANT.Definitions;
using MySqlConnector;
using static System.Net.Mime.MediaTypeNames;

//The voice someone calls (in the labyrinth)
namespace HYDRANT;

/// <summary>
/// Access API via MySQL
/// </summary>
public class SQL
{
	private string Connection;
	/// <summary>
	/// MySQL Connection String
	/// </summary>
	/// <param name="connectionString">SQL Connection String</param>
	public SQL(string connectionString)
	{
		//Connect to the DB and open the connection
		Connection = connectionString;
	}
    
    /// <summary>
    /// Gets article via it's URL.
    /// </summary>
    /// <param name="URL">URL to get Article from</param>
    /// <returns>Article Object if found, null if not.</returns>
    public Article? GetArticleFromURL(string URL)
    {
        //Run command
        MySqlConnection DB = new() { ConnectionString = Connection };
        DB.Open();
        
        MySqlCommand cmd = new($"SELECT * FROM ARTICLES WHERE URL=@URL;", DB);
        cmd.Parameters.AddWithValue("@URL", URL);
        MySqlDataReader reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            //'deserialize' into article
            Article article = new()
            {
                Url = reader["URL"].ToString() ?? string.Empty,
                Title = reader["TITLE"].ToString() ?? "",
                PublishDate = Convert.ToDateTime(reader["PUBLISH_DATE"]),
                IsPaywall = Convert.ToBoolean(reader["PAYWALL"] ?? true),
                Summary = reader["SUMMARY"].ToString()!,
                ImageURL = reader["ImageURL"].ToString(),
                Text = reader["ARTICLE_TEXT"].ToString() ?? "Article Text unavailable.",
                PublisherID = (int)reader["PUBLISHER_ID"],
                Business = Convert.ToBoolean(reader["BUSINESS_RELATED"]),
                CompaniesMentioned = reader["COMPANIES_MENTIONED"].ToString()!,
                Author = reader["AUTHOR"].ToString()!,
                Sectors = reader["SECTORS"].ToString()!,
                TimeToRead = Convert.ToInt32(reader["TimeToRead"].ToString()),
                Impact = Convert.ToInt32(reader["IMPACT"]),
            };
            return article;
        }
        
        //No article found.
        return null;
    }


    /// <summary>
    /// Gets articles from database
    /// </summary>
    /// <param name="Limit">How many articles to fetch</param>
    /// <param name="Offset">Skip this amount of articles first</param>
    /// <param name="Filter">SQL filter</param>
    /// <param name="Minimal">Only returns required stuff</param>
    /// <param name="AllowUserSubmittedArticles">Disables user submitted articles
    /// (overriden if UGC is mentioned)</param>
    /// <param name="PublisherID">Filter by PublisherID</param>
    /// <returns>JSON Formatted list of articles.</returns>
	public List<Article> GetArticles(int Limit = 0, int Offset = 0,
	string Filter = "ORDER BY PUBLISH_DATE DESC", bool Minimal = true,
    bool AllowUserSubmittedArticles = false, int PublisherID = -1)
     {
        string query;
        //Select queries, minimal mode is much quicker for large queries.
        if (Minimal)
        {
            query = @"SELECT URL, TITLE, PUBLISH_DATE, Impact,
		    SUMMARY, ImageURL, PUBLISHER_ID, AUTHOR,TimeToRead";
        }
        else
        {
            query = @"SELECT URL, TITLE, PUBLISH_DATE, Impact,
		    PAYWALL, SUMMARY, ImageURL,ARTICLE_TEXT, PUBLISHER_ID, BUSINESS_RELATED,
		    COMPANIES_MENTIONED, AUTHOR, SECTORS,TimeToRead";
        }
        
        //Disable UGC (overriden if UGC is mentioned)
        if (!AllowUserSubmittedArticles && !Filter.Contains("UserGeneratedArticle"))
        {
            if (string.IsNullOrWhiteSpace(Filter) || Filter.Split(" ")[0] == "ORDER")
            { Filter = "WHERE UserGeneratedArticle = 0 " + Filter; }
            else if (Filter.Contains("ORDER"))
            {
                Filter = Filter.Replace("ORDER", "AND UserGeneratedArticle = 0 ORDER");
            }
            else { Filter += " AND UserGeneratedArticle = 0"; }
        }
        
        //Filter by publisher if enabled
        if (PublisherID != -1)
        {
            if (string.IsNullOrWhiteSpace(Filter) || Filter.Split(" ")[0] == "ORDER")
            { Filter = "WHERE PUBLISHER_ID = 0 " + Filter; }
            else if (Filter.Contains("ORDER"))
            {
                Filter = Filter.Replace("ORDER", $" AND PUBLISHER_ID = {PublisherID} ORDER");
            }
            else { Filter += $" AND PUBLISHER_ID = {PublisherID}"; }
        }

        //Add common filtering stuff
        query += $" FROM ARTICLES {Filter} LIMIT {Limit} OFFSET {Offset};";
            
        //Run command
        MySqlConnection DB = new() { ConnectionString = Connection };
		DB.Open();

		MySqlCommand cmd = new(query, DB);
		MySqlDataReader reader = cmd.ExecuteReader();
		List<Article> articles = new();
		while (reader.Read())
		{
            //'deserialize' into article and add to collection
            articles.Add(DeserializeReader(reader));
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
	public List<string> GetURLs()
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
    /// <param name="UserGenerated">Flag article as user submitted</param>
    public void UploadArticle(Article Article, bool UserGenerated = false)
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
                (TITLE, URL, ARTICLE_TEXT, PUBLISH_DATE, SUMMARY, IMPACT,
				BUSINESS_RELATED, PAYWALL, COMPANIES_MENTIONED, 
				EXECUTIVES_MENTIONED, ImageURL, PUBLISHER_ID, AUTHOR, SECTORS,
                UserGeneratedArticle, pubname, TimeToRead) 
                VALUES 
                (@Title, @Url, @Content, @PublishDate, @Summary, @IMPACT,
				@BusinessRelated, @Paywall, @CompaniesMentioned, 
				@ExecutivesMentioned, @ImageURL, @PUBLISHER_ID, @AUTHOR, @SECTORS,
                @UserGeneratedArticle, @pubname, @TimeToRead)";
					command.Parameters.AddWithValue("@Title", Article.Title);
					command.Parameters.AddWithValue("@Url", Article.Url);
					command.Parameters.AddWithValue("@Content", Article.Text);
					command.Parameters.AddWithValue("@PublishDate", Article.PublishDate);
					command.Parameters.AddWithValue("@Summary", Article.Summary);
					command.Parameters.AddWithValue("@IMPACT", Article.Impact);
					command.Parameters.AddWithValue("@BusinessRelated", Article.Business);
					command.Parameters.AddWithValue("@Paywall", Article.IsPaywall);
					command.Parameters.AddWithValue("@CompaniesMentioned", Article.CompaniesMentioned);
					command.Parameters.AddWithValue("@ExecutivesMentioned", "");
					command.Parameters.AddWithValue("@ImageURL", Article.ImageURL);
					command.Parameters.AddWithValue("@PUBLISHER_ID", Article.PublisherID);
					command.Parameters.AddWithValue("@AUTHOR", Article.Author);
					command.Parameters.AddWithValue("@SECTORS", Article.Sectors);
					command.Parameters.AddWithValue("@UserGeneratedArticle", UserGenerated);
					command.Parameters.AddWithValue("@pubname", Article.PublicationName ?? "Unknown Publication");
					command.Parameters.AddWithValue("@TimeToRead", Article.TimeToRead ?? 0);
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
    public void UpdateArticle(Article article)
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
                SUMMARY = @Summary, 
                Impact = @Impact,
                HEADLINE = @Headline, 
                BUSINESS_RELATED = @BusinessRelated, 
                PAYWALL = @Paywall, 
                COMPANIES_MENTIONED = @Companies, 
                EXECUTIVES_MENTIONED = @Execs, 
                ImageURL = @ImageURL, 
                PUBLISHER_ID = @PUBLISHER_ID, 
                AUTHOR = @AUTHOR, 
                SECTORS = @SECTORS,
                WHERE URL = @Url";

                    command.Parameters.AddWithValue("@Title", article.Title);
                    command.Parameters.AddWithValue("@Url", article.Url);
                    command.Parameters.AddWithValue("@Content", article.Text);
                    command.Parameters.AddWithValue("@PublishDate", article.PublishDate);
                    command.Parameters.AddWithValue("@Summary", article.Summary);
                    command.Parameters.AddWithValue("@Weighting", article.Impact);
                    command.Parameters.AddWithValue("@BusinessRelated", article.Business);
                    command.Parameters.AddWithValue("@Paywall", article.IsPaywall);
                    command.Parameters.AddWithValue("@Companies", article.CompaniesMentioned);
                    command.Parameters.AddWithValue("@Execs", article.ExecsMentioned);
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

    /// <summary>
    /// Flags article as clickbait.
    /// </summary>
    /// <param name="URL">URL of article that's clickbait.</param>
    public void IncrementClickbaitCounter(string URL)
    {
        try
        {
            using (MySqlConnection connection = new(Connection))
            {
                connection.Open();
                
                using (MySqlCommand cmd = new("IncrementClickbaitCount", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_url", URL);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    /// <summary>
    /// Flags article summary as bad.
    /// </summary>
    /// <param name="URL">URL of article that's bad.</param>
    public void IncrementSummaryReportCounter(string URL)
    {
        try
        {
            using (MySqlConnection connection = new(Connection))
            {
                connection.Open();
                
                using (MySqlCommand cmd = new("IncrementSummaryReportCount", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_url", URL);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    /// <summary>
    /// Gets article text for provided URL
    /// </summary>
    /// <param name="URL">URL for article text</param>
    /// <returns>Article Text</returns>
    public string GetArticleText(string URL)
    {
        try
        {
            using MySqlConnection connection = new(Connection);
            connection.Open();
            
            using MySqlCommand cmd = new("SELECT ARTICLE_TEXT, PAYWALL FROM ARTICLES WHERE URL = @url",
                connection);
            cmd.Parameters.AddWithValue("@url", URL);
            var r = cmd.ExecuteReader();
            string ArticleText = "Article Text Unavailable";
            while (r.Read())
            {
                //Can't return paywalled content, or base64 encoded content.
                if (!bool.Parse(r["PAYWALL"].ToString() ?? "False")&&
                    !r["ARTICLE_TEXT"].ToString().Contains("base64:"))
                {
                    ArticleText = r["ARTICLE_TEXT"].ToString() ?? "Article Text Unavailable";
                }
                else
                {
                    ArticleText = "Article Text isn't available for this article";
                }
            }
            
            return ArticleText;
        }
        catch (Exception e)
        {
            return "Failed to fetch article text\n" + e.Message ;
        }
    }

    /// <summary>
    /// Turns a MySQLDataReader to an Article Object
    /// </summary>
    /// <param name="reader">MySQLDataReader</param>
    /// <returns>Article Object</returns>
    public Article DeserializeReader(MySqlDataReader reader)
    {
        Article article = new()
        {
            Url = reader["URL"].ToString() ?? string.Empty,
            Title = reader["TITLE"].ToString() ?? "",
            Impact = Convert.ToInt32(reader["Impact"]),
            PublishDate = Convert.ToDateTime(reader["PUBLISH_DATE"]),
            Summary = reader["SUMMARY"].ToString()!,
            ImageURL = reader["ImageURL"].ToString(),
            PublisherID = (int)reader["PUBLISHER_ID"],
            Author = reader["AUTHOR"].ToString()!,
            TimeToRead = Convert.ToInt32(reader["TimeToRead"]),
        };
        
        
        if (reader.GetSchemaTable().Columns.Contains("PAYWALL"))
        {
            article.IsPaywall = Convert.ToBoolean(reader["PAYWALL"] ?? true);
            article.Text = reader["ARTICLE_TEXT"].ToString() ?? "Article Text unavailable.";
            article.Business = Convert.ToBoolean(reader["BUSINESS_RELATED"]);
            article.CompaniesMentioned = reader["COMPANIES_MENTIONED"].ToString()!;
            article.Sectors = reader["SECTORS"].ToString()!;
            
        }
        return article;
    }

    /// <summary>
    /// Searches every article in 
    /// </summary>
    /// <param name="SearchTerm"></param>
    /// <returns></returns>
    public List<Article> SearchArticles(string SearchTerm)
    {
        string query = @"
            SELECT * FROM Articles
            WHERE ARTICLE_TEXT LIKE @searchTerm OR
                  TITLE LIKE @searchTerm OR
                  URL LIKE @searchTerm OR
                  SUMMARY LIKE @searchTerm LIMIT 50";
        
        List<Article> articles = new();
        using (MySqlConnection connection = new(Connection))
        {
            using (MySqlCommand command = new(query, connection))
            {
                // Add the parameter and define its value
                command.Parameters.AddWithValue("@searchTerm", "%" + SearchTerm + "%");
                
                try
                {
                    // Open the connection
                    connection.Open();
                    
                    // Execute the query and get the results
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Loop through the results
                        while (reader.Read())
                        {
                            // 'Deserialize' the reader into an article object
                            Article article = DeserializeReader(reader);
                            // Add the article to the list
                            articles.Add(article);  
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
        //return articles
        return articles;
    }
    
    
    /// <summary>
    /// Adds a view to the article
    /// </summary>
    /// <param name="URL"></param>
    public void AddView(string URL)
    {
        try
        {
            using (MySqlConnection connection = new(Connection))
            {
                connection.Open();
                
                using (MySqlCommand cmd = new("AddView", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("article_url", URL);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

}
