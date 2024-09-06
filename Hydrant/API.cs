using HYDRANT.Definitions;
using System.Text.Json;
using HYDRANT.Serializers;
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
    
    /// <summary>
    /// Gets Articles from API
    /// </summary>
    /// <param name="limit">How many articles you want to set</param>
    /// <param name="Offset">Skip this amount of articles</param>
    /// <param name="token">Used to access certain content</param>
    /// <param name="FilterName">Name of the filter</param>
    /// <param name="Minimal">Minimal Mode (less data is returned)</param>
    /// <param name="Publishers">Only return filters from these publisher IDs</param>
    /// <returns>List of articles if successful.</returns>
    public async Task<List<Article>?> GetArticles(int limit = 20, int Offset = 0, 
        string? token = null, string FilterName = "Latest", bool Minimal = false,
        string Publishers = "")
	{
		var endpoint = $"/Articles/GetArticles?limit={limit}&offset={Offset}&" +
                       $"FilterName={FilterName}&minimal={Minimal}" +
                       $"&token={token}&publishers={Publishers}";
		using HttpClient client = new();

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
        string URL = $"{Endpoint}/Articles/ReportAsClickbait.URL={Uri.EscapeDataString(Article.URL)}";
        await client.GetAsync(URL);
    }

    /// <summary>
    /// Reports an article summary as incorrect.
    /// </summary>
    /// <param name="Article">Article to report</param>
    public async Task ReportArticle(Article Article)
    {
        using HttpClient client = new();
        string URL = $"{Endpoint}/Articles/ReportArticleSummary?Articl.URL={Uri.EscapeDataString(Article.URL)}";
        await client.GetAsync(URL);
    }
    
    /// <summary>
    /// Gets article text
    /// </summary>
    /// <param name=.URL">Article to report</param>
    public async Task<string> GetArticleText(string URL)
    {
        using HttpClient client = new();
        URL = $"{Endpoint}/Articles/GetArticleText&URL={Uri.EscapeDataString(URL)}";
        HttpResponseMessage Response = await client.GetAsync(URL);
        return await Response.Content.ReadAsStringAsync();
    }
    
    /// <summary>
    /// Gets all filters available for a user
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<List<Filter>> GetFilters(string token = null)
    {
        string URL = Endpoint + "/Filter/GetFilters";
        using (HttpClient client = new HttpClient())
        {
            var requestURL = string.IsNullOrEmpty(token) ? URL : $"{URL}?Token={token}";
            
            HttpResponseMessage response = await client.GetAsync(requestURL);
            
            if (response.IsSuccessStatusCode)
            {
                
                string content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Filter>>(content)!;
            }
            else
            {
                return null;
            }
        }
    }
    

    /// <summary>
    /// Searches an article for a SearchString
    /// </summary>
    /// <param name="SearchString">Text to search for</param>
    /// <returns>List of articles.</returns>
    public async Task<List<Article>> Search(string SearchString)
    {
        string URL = Endpoint + $"/Articles/Search?SearchString={SearchString}";
        using (HttpClient client = new())
        {
            
            HttpResponseMessage response = await client.GetAsync(URL);
            
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var articles = JsonSerializer.Deserialize<List<Article>>(content,
                    ArticleSerialiser.Default.ListArticle)!;
                return articles;
            }
        }
        
        return new();
    }

    /// <summary>
    /// Bumps the.URL by one.
    /// </summary>
    /// <param name=.URL">Text to search for</param>
    /// <returns>List of articles.</returns>
    public async Task AddView(string URL)
    {
        URL = Endpoint + $"/Articles/AddView.URL={URL}";
        using (HttpClient client = new())
        {
            await client.GetAsync(URL);
        }
    }

}
