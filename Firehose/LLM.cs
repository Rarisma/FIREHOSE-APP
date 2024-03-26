 using System.Text;
using System.Text.Json;
//Till the streets paved in gold.
namespace Firehose;
    public class LLM
    {
        public static async Task<string> OneShot(List<Message> messages, double temperature=0.2, int maxTokens=-1, bool stream=false)
        {
            try
            {
                var payload = new { messages, temperature, max_tokens = maxTokens, stream };

                //Post query to LLM Server
                var response = await new HttpClient().PostAsync("http://localhost:8080/v1/chat/completions", 
                    new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
                    new CancellationTokenSource(TimeSpan.FromSeconds(100)).Token).ConfigureAwait(false);

                // Get result if we didn't get an error
                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                    {
                        //Wade through JSON bs and get choices value.
                        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                    }
                }

                return $"Error: {response.StatusCode}";
            }
            catch (Exception ex) { return $"Error: {ex.Message}";  }
        }
    }

public class Message
{
    public string role { get; set; }
    public string content { get; set; }
}
