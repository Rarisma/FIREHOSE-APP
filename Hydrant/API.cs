using HYDRANT.Definitions;
using System.Text.Json;
//Welcome to the style you haven't seen in a while, It's lavish
namespace HYDRANT;
/// <summary>
/// Access the FirehoseServer
/// </summary>
public class API
{
	/// <summary>
	/// Creates a new object for accessing the API
	/// </summary>
	/// <param name="endpoint">Base endpoint, leave blank to use hallon.rarisma.net</param>
	public API(string endpoint = "https://www.hallon.rarisma.net:5000")
	{
		Endpoint = endpoint;
		//API_KEY = apiKey;
	}

	/// <summary>
	/// API Endpoint to access server.
	/// </summary>
	public string Endpoint;

	///// <summary>
	///// Set to your API Key.
	///// </summary>
	//public string API_KEY;

	/// <summary>
	/// Gets Articles from API
	/// </summary>
	/// <param name="Limit">How many articles you want to set</param>
	/// <param name="Offset">Skip this amount of articles</param>
	/// <param name="Filter">Filter articles via SQL</param>
	/// <param name="Minimal">Minimal Mode</param>
	/// <returns>List of articles if successful.</returns>
	public async Task<List<Article>?> GetArticles(int Limit = 20, int Offset = 0, 
        string Filter = "ORDER BY PUBLISH_DATE DESC", bool Minimal=true)
	{
		var endpoint = $"/Articles/GetArticles?limit={Limit}&offset={Offset}&filter={Filter}&minimal={Minimal}";
		using HttpClient client = new();
		//client.DefaultRequestHeaders.Add("ApiKey", API_KEY);

		//Make request and check it was successful
		var response = await client.GetAsync(Endpoint + endpoint);
		response.EnsureSuccessStatusCode();

		//Deserialize JSON content
		var content = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<List<Article>>(content)!;
	}

	/// <summary>
	/// Loads information about publishers from API
	/// </summary>
	public async Task<List<Publication>> GetPublications()
	{
        // Send a GET req
        using HttpClient client = new();
        //client.DefaultRequestHeaders.Add("ApiKey", API_KEY);
        var response = await client.GetAsync($"{Endpoint}/Publication/GetPublicationData");
        
        if (response.IsSuccessStatusCode)
        {
            // Read the response content as a string (if needed)
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Publication>>(content)!;
        }

        //return new list if something went wrong.
        return new();
	}
    /// <summary>
    /// Report that an article is clickbait
    /// </summary>
    /// <param name="Article">Article to report</param>
    /// <returns></returns>
    public async Task VoteClickbait(Article Article)
    {
        using HttpClient client = new();
        //client.DefaultRequestHeaders.Add("ApiKey", API_KEY);
        string url = $"{Endpoint}/Articles/ReportAsClickbait?URL={Uri.EscapeDataString(Article.Url)}";
        await client.GetAsync(url);
    }

    /// <summary>
    /// Reports an article summary as incorrect.
    /// </summary>
    /// <param name="Article">Article to report</param>
    public async Task ReportArticle(Article Article)
    {
        using HttpClient client = new();
        string url = $"{Endpoint}/Articles/ReportArticleSummary?ArticleURL={Uri.EscapeDataString(Article.Url)}";
        await client.GetAsync(url);
    }
}
