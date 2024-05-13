using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace FirehoseServer;

/// <summary>
/// This is a port of IAI AIToolkit.
/// This is fairly straight forward implementation of their
/// LLMTools.py class with some QoL Tweaks.
/// </summary>
public static class LLM
{
    public static string[] Endpoints =
    [
        "http://localhost:6666/v1/chat/completions",
        "http://localhost:6666/v1/chat/completions"
    ];

    public static string[] ModelNames =
    [
        "lmstudio-community/gemma-1.1-2b-it-GGUF/gemma-1.1-2b-it-Q8_0.gguf",
        "lmstudio-community/gemma-1.1-2b-it-GGUF/gemma-1.1-2b-it-Q8_0.gguf:2"
    ];

    /// <summary>
    /// Asks the LLM a one-shot question.
    ///  i.e. you don't get a response.
    /// </summary>
    /// <param name="messages">Provide a list of messages, include a
    /// system prompt and a user prompt.</param>
    /// <param name="temperature">
    /// Lower values i.e. 0.2 make it more grounded in reality,
    /// Higher ones like 0.8 make it more creative and unhinged.
    /// </param>
    /// <param name="maxTokens"></param>
    /// <param name="stream"></param>
    /// <returns>The models thoughts, feelings and opinions.</returns>
    public static async Task<string> SendPostRequest(string systemPrompt, string userMessage, int ID)
    {
        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            temperature = 0.2,
            model = ModelNames[ID]
        };

        try
        {
            using (var httpClient = new HttpClient())
            {
                // Serialize the request body to JSON
                var jsonContent = JsonSerializer.Serialize(requestBody);
                using (var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
                {
                    // Add necessary headers
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Send the POST request
                    var response = await httpClient.PostAsync(Endpoints[ID], content);
                    // Ensure we got a successful response
                    response.EnsureSuccessStatusCode();

                    using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                    {
                        //Wade through JSON bs and get choices value.
                        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content")
                            .GetString();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "Summary Unavailable.";
        }
    }
}

    /// <summary>
    /// This represents a JSON message object from an LLM Response.
    /// </summary>
    public class Message
{
    /// <summary>
    /// This the 'sender' of message.
    /// System - This is the system prompt.
    /// Assistant - This is the LLM speaking.
    /// User - This is the user speaking.
    /// </summary>
    public string role;

    /// <summary>
    /// This is body of the message.
    /// </summary>
    public string content;
}


[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(List<Message>))]
public partial class MessageSerialiser : JsonSerializerContext
{
}
