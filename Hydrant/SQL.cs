using HYDRANT.Definitions;
using MySqlConnector;
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
                Summary = reader["SUMMARY"].ToString()!,
                ImageURL = reader["ImageURL"].ToString(),
                Text = reader["ARTICLE_TEXT"].ToString() ?? "Article Text unavailable.",
                PublisherID = (int)reader["PUBLISHER_ID"],
                Author = reader["AUTHOR"].ToString()!,
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
    /// <param name="Publishers">Filter by PublisherID</param>
    /// <returns>JSON Formatted list of articles.</returns>
	public List<Article> GetArticles(int Limit = 0, int Offset = 0,
	string Filter = "ORDER BY PUBLISH_DATE DESC", bool Minimal = true,
    bool AllowUserSubmittedArticles = false, int[]? Publishers = null)
     {
        string query;
        //Select queries, minimal mode is much quicker for large queries.
        query = @"SELECT URL, TITLE, PUBLISH_DATE, Impact,
		    SUMMARY, ImageURL, PUBLISHER_ID, AUTHOR, TimeToRead";

        if (!Minimal) { query += ", ARTICLE_TEXT"; }
        
        //Filter by publisher if enabled
        if (Publishers == null || Publishers.Length != 0)
        {
            var pub_filter = $"PUBLISHER_ID IN ({string.Join(",", Publishers)})";
            if (string.IsNullOrWhiteSpace(Filter) || Filter.Split(" ")[0] == "ORDER")
            { Filter = $"WHERE {pub_filter} {Filter}"; }
            else if (Filter.Contains("ORDER"))
            {
                Filter = Filter.Replace("ORDER", $" AND {pub_filter} ORDER");
            }
            else { Filter += $" AND {pub_filter}"; }
        }
        else //Block Testing Data from showings and UGC articles.
        {
            if (string.IsNullOrWhiteSpace(Filter) || Filter.Split(" ")[0] == "ORDER")
            { Filter = "WHERE NOT PUBLISHER_ID = 0 " + Filter; }
            else if (Filter.Contains("ORDER"))
            {
                Filter = Filter.Replace("ORDER", " AND NOT PUBLISHER_ID = 0 ORDER");
            }
            else { Filter += " NOT AND PUBLISHER_ID = 0"; }
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
    public void UploadArticle(Article Article)
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
				ImageURL, PUBLISHER_ID, AUTHOR, TimeToRead, Tags) 
                VALUES 
                (@Title, @Url, @Content, @PublishDate, @Summary, @IMPACT,
				@ImageURL, @PUBLISHER_ID, @AUTHOR, @TimeToRead, @Tags)";
					command.Parameters.AddWithValue("@Title", Article.Title);
					command.Parameters.AddWithValue("@Url", Article.Url);
					command.Parameters.AddWithValue("@Content", Article.Text);
					command.Parameters.AddWithValue("@PublishDate", Article.PublishDate);
					command.Parameters.AddWithValue("@Summary", Article.Summary);
					command.Parameters.AddWithValue("@IMPACT", Article.Impact);
					command.Parameters.AddWithValue("@ImageURL", Article.ImageURL);
					command.Parameters.AddWithValue("@PUBLISHER_ID", Article.PublisherID);
					command.Parameters.AddWithValue("@AUTHOR", Article.Author);
					command.Parameters.AddWithValue("@TimeToRead", Article.TimeToRead ?? 0);
					command.Parameters.AddWithValue("@Tags", Article.Tags ?? "");
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
                ImageURL = @ImageURL, 
                PUBLISHER_ID = @PUBLISHER_ID, 
                AUTHOR = @AUTHOR, 
                WHERE URL = @Url";

                    command.Parameters.AddWithValue("@Title", article.Title);
                    command.Parameters.AddWithValue("@Url", article.Url);
                    command.Parameters.AddWithValue("@Content", article.Text);
                    command.Parameters.AddWithValue("@PublishDate", article.PublishDate);
                    command.Parameters.AddWithValue("@Summary", article.Summary);
                    command.Parameters.AddWithValue("@Weighting", article.Impact);
                    command.Parameters.AddWithValue("@ImageURL", article.ImageURL);
                    command.Parameters.AddWithValue("@PUBLISHER_ID", article.PublisherID);
                    command.Parameters.AddWithValue("@AUTHOR", article.Author);

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
    /// <param name="url">URL for article text</param>
    /// <param name="publications"></param>
    /// <returns>Article Text</returns>
    public string GetArticleText(string url, List<Publication> publications)
    {
        try
        {
            using MySqlConnection Connection = new(this.Connection);
            Connection.Open();
            
            using MySqlCommand Cmd = new("SELECT ARTICLE_TEXT, PUBLISHER_ID FROM ARTICLES WHERE URL = @url",
                Connection);
            Cmd.Parameters.AddWithValue("@url", url);
            var R = Cmd.ExecuteReader();
            string ArticleText = "Article Text Unavailable";
            while (R.Read())
            {
                //Can't return paywalled content, or base64 encoded content.
                if (!publications[Convert.ToInt16(R["PUBLISHER_ID"])].Paywall &&
                    !R["ARTICLE_TEXT"].ToString().Contains("base64:"))
                {
                    ArticleText = R["ARTICLE_TEXT"].ToString() ?? "Article Text Unavailable";
                }
                else
                {
                    ArticleText = "Article Text isn't available for this article";
                }
            }
            
            return ArticleText;
        }
        catch (Exception E)
        {
            return "Failed to fetch article text\n" + E.Message ;
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
        
        
        if (reader.GetSchemaTable().Columns.Contains("ARTICLE_TEXT"))
        {
            article.Text = reader["ARTICLE_TEXT"].ToString() ?? "Article Text unavailable.";
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
