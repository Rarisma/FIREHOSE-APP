using HYDRANT.Definitions;
using System.Text.Json;
using System.Net.Http;
using System.Runtime.Intrinsics.Wasm;
using System.Threading.Tasks;

namespace HYDRANT
{
    /// <summary>
    /// Access the FirehoseServer
    /// </summary>
    public class API : IDisposable
    {

        private readonly HttpClient _client;
        public string Endpoint { get; }

        /// <summary>
        /// Creates a new object for accessing the API
        /// </summary>
        /// <param name="endpoint">Base endpoint, leave blank to use hallon.rarisma.net</param>
        /// <param name="apiKey">Optional API Key for authentication</param>
        public API(
#if DEBUG
            string endpoint = "https://www.hallon.rarisma.net:5001",
#else
            string endpoint = "https://www.hallon.rarisma.net:5000",
#endif
            string? apiKey = null)
        
        {
            Endpoint = endpoint;
            _client = new HttpClient
            {
                BaseAddress = new Uri(Endpoint)
            };
            if (!string.IsNullOrEmpty(apiKey))
            {
                _client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            }
        }

        /// <summary>
        /// Gets Articles from API
        /// </summary>
        public async Task<List<Article>?> GetArticles(int limit = 200, int Offset = 0,
         string FilterName = "Latest", string SearchTerm = "", string Publishers = "")
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"limit={limit}",
                    $"offset={Offset}",
                    $"FilterName={Uri.EscapeDataString(FilterName)}",
                    $"SearchTerm={Uri.EscapeDataString(SearchTerm ?? "")}",
                    $"publishers={Uri.EscapeDataString(Publishers)}"
                };

                var endpoint = $"/Articles/GetArticles?{string.Join("&", queryParams)}";
                var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<List<Article>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                // Log exception
                // Optionally rethrow or return null
                throw;
            }
        }
        
        
        /// <summary>
        /// Gets initialisation data
        /// (Replaces GetFilters and GetPublications)
        /// </summary>
        /// <returns></returns>
        public async Task<Data> GetData()
        {
            var endpoint = "/Data/GetData";
            var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<Data>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        /// <summary>
        /// Report that an article is clickbait
        /// </summary>
        public async Task VoteClickbait(string articleURL)
        {
            try
            {
                var endpoint = $"/Articles/ReportClickbait?URL={Uri.EscapeDataString(articleURL)}";
                var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Log exception
                throw;
            }
        }

        /// <summary>
        /// Reports an article summary as incorrect.
        /// </summary>
        public async Task ReportArticle(string articleURL)
        {
            try
            {
                var endpoint = $"/Articles/ReportSummary?URL={Uri.EscapeDataString(articleURL)}";
                var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Log exception
                throw;
            }
        }

        /// <summary>
        /// Bumps the view count for an article.
        /// </summary>
        public async Task AddView(string URL)
        {
            try
            {
                var endpoint = $"/Articles/AddView?URL={Uri.EscapeDataString(URL)}";
                var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Log exception
                throw;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
