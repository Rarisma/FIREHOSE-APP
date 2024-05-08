using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//Egg Foo Young, no egg.
namespace HYDRANT;
public class Publication
{
    [JsonProperty("ID")]
    public int ID { get; set; }

    [JsonProperty("BaseWeight")]
    public int Weighting { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Favi")]
    public string Favicon { get; set; }

    [JsonProperty("AlwaysJakeFlag")]
    public bool AlwaysJakeFlag { get; set; }

    [JsonProperty("URLs")]
    public string[] URLs { get; set; }

    public static List<Publication> LoadFromJSON(string File = "Data/Feeds.json")
    {
        // Read the JSON file content
        string json = System.IO.File.ReadAllText(File);
        return JsonConvert.DeserializeObject<List<Publication>>(JObject.Parse(json)["Publications"].ToString());
    }

    public static async Task<List<Publication>> LoadFromAPI()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Send a GET request to the specified URI
                HttpResponseMessage response = await client.GetAsync($"{Article.HallonEndpoint}/Publication/GetPublicationData");

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string (if needed)
                    string content = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Publication>>(content);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Exception caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }
    }
}

