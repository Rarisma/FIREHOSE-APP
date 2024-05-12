using HYDRANT.Definitions;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;
//The voice someone calls (in the labyrinth)
[assembly: InternalsVisibleTo("FirehoseServer")]
namespace Hydrant;

/// <summary>
/// Hey I know what your doing.
/// stop fucking decompiling this  there's nothing to find.
///
/// you don't have access to this because sql db is firewalled.
///
/// https://www.youtube.com/watch?v=7hpFYz45Odg
/// </summary>
internal class SQL
{
	private const string lmao = "Hi Callum, I knew you'd do this. Say hi!";
	private string Connection;
	/// <summary>
	/// MySQL Connection String
	/// </summary>
	/// <param name="ConnectionString"></param>
	public SQL(string ConnectionString)
	{
		//Connect to the DB and open the connection
		Connection = ConnectionString;
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
		COMPANIES_MENTIONED, JAKE_FLAG, AUTHOR FROM ARTICLES {Filter} LIMIT {Limit}
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
				Url = reader["URL"].ToString(),
				Title = reader["TITLE"].ToString(),
				RSSSummary = reader["RSS_SUMMARY"].ToString(),
				PublishDate = Convert.ToDateTime(reader["PUBLISH_DATE"]),
				IsHeadline = Convert.ToBoolean(reader["HEADLINE"]),
				IsPaywall = Convert.ToBoolean(reader["PAYWALL"]),
				Summary = reader["SUMMARY"].ToString(),
				ImageURL = reader["ImageURL"].ToString(),
				Text = reader["ARTICLE_TEXT"].ToString(),
				PublisherID = (int)reader["PUBLISHER_ID"],
				Business = Convert.ToBoolean(reader["BUSINESS_RELATED"]),
				HasJakeFlag = Convert.ToBoolean(reader["JAKE_FLAG"]),
				CompaniesMentioned = reader["COMPANIES_MENTIONED"].ToString(),
				Author = reader["AUTHOR"].ToString(),
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
		while (reader.Read()) { URLs.Add(reader["URL"].ToString()); }
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
				EXECUTIVES_MENTIONED, JAKE_FLAG, ImageURL, PUBLISHER_ID, AUTHOR) 
                VALUES 
                (@Title, @Url, @Content, @PublishDate, @RssSummary, @Summary, @Weighting,
				@Headline, @BusinessRelated, @Paywall, @CompaniesMentioned, 
				@ExecutivesMentioned, @JakeFlag, @ImageURL, @PUBLISHER_ID, @AUTHOR)";
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
					command.Parameters.AddWithValue("@JakeFlag", Article.HasJakeFlag);
					command.Parameters.AddWithValue("@ImageURL", Article.ImageURL);
					command.Parameters.AddWithValue("@PUBLISHER_ID", Article.PublisherID);
					command.Parameters.AddWithValue("@AUTHOR", Article.Author);
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

}