using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

//The pretty good seal
namespace Firehose2;

/// <summary>
/// This is a port of IAI AIToolkit.
/// This is fairly straight forward implementation of their
/// LLMTools.py class with some QoL Tweaks.
/// </summary>
public static class LLM
{
    private const string Endpoint = "http://localhost:8080/v1/chat/completions";

    /// <summary>
    /// Launches an instance of LLM Server.
    /// </summary>
    public static void LaunchServer()
    {
        //Check there isn't an LLM Server already running
        if (IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
                .Count(listener => listener.Port == 8080) > 0)
        {
            Console.WriteLine("LLM Server is already running.");
            return;
        }

        //Spawn process.
        Console.WriteLine("Couldn't find an instance of LLM Server, spawning one.");
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/k python3 -m llama_cpp.server --config_file Data/model_config.cfg",
            UseShellExecute = true
        });
    }

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
    public static async Task<string> SendPostRequest(string systemPrompt, string userMessage)
    {
        var requestBody = new
        {
            messages = new[]
            {
                new { content = systemPrompt, role = "system" },
                new { content = userMessage, role = "user" }
            },
            temperature = 0.2
        };

        try
        {
            using (var httpClient = new HttpClient())
            {
                // Serialize the request body to JSON
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                using (var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
                {
                    // Add necessary headers
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Send the POST request
                    var response = await httpClient.PostAsync(Endpoint, content);

                    // Ensure we got a successful response
                    response.EnsureSuccessStatusCode();

                    // Read and return the response body
                    using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                    {
                        //Wade through JSON bs and get choices value.
                        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                    }
                }
            }
        }
        catch (HttpRequestException e)
        {
            // Handle the exception as needed
            return $"Exception Caught! Message: {e.Message}";
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
