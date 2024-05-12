using System.Text;

namespace FirehoseServer;

public class RequestResponseLoggingMiddleware
{
	private readonly RequestDelegate _next;
	private static readonly object _lock = new object();
	private const string LogFilePath = "Logs.txt";

	public RequestResponseLoggingMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var ipAddress = context.Connection.RemoteIpAddress?.ToString();
		var requestLog = $"Request from IP {ipAddress} - {context.Request.Method}: " +
		                 $"{context.Request.Path.Value} at {DateTime.UtcNow} " +
		                 $"QueryString: {context.Request.QueryString}";
		WriteToFile(requestLog);

		var originalBodyStream = context.Response.Body;
		using (var responseBody = new MemoryStream())
		{
			context.Response.Body = responseBody;

			await _next(context);

			responseBody.Seek(0, SeekOrigin.Begin);
			var text = await new StreamReader(responseBody).ReadToEndAsync();
			var responseLog = $"Response: {context.Response.StatusCode} at {DateTime.UtcNow}, Body: {text}";
			WriteToFile(responseLog);

			responseBody.Seek(0, SeekOrigin.Begin);
			await responseBody.CopyToAsync(originalBodyStream);
		}
	}

	private void WriteToFile(string message)
	{
		lock (_lock)
		{
			using (var streamWriter = new StreamWriter(LogFilePath, true, Encoding.UTF8))
			{
				streamWriter.WriteLine(message);
			}
		}
	}
}