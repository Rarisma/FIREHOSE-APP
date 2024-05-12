using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FirehoseServer;

public class ApiKeyMiddleware
{
	private readonly RequestDelegate _next;
	private readonly List<string> _keys = new()
	{
		"l3_7jnpSlnrplu_Ocdpb2A6I50xD_OVR59mHIdeFp_k=", //Scott api key
		"bbjG_POHaWvmMwobKge82Jy3WRXlHhnwGfpuJmN2rBY=", // firehose API Key
		"gYW-rhYS6GhqohpTjRbwmLVQsThH29HDIeReVWpEjm8=" // Newsapp API key
	};

	public ApiKeyMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Bypass API key check for Swagger routes
		if (context.Request.Path.StartsWithSegments("/swagger")
		    || context.Request.Path == "/")
		{
			await _next(context);
			return;
		}



		const string APIKEY_NAME = "ApiKey";
		if (!context.Request.Headers.TryGetValue(APIKEY_NAME, out var extractedApiKey))
		{
			context.Response.StatusCode = 401; // Unauthorized
			await context.Response.WriteAsync("API Key is missing");
			return;
		}

		if (!_keys.Contains(extractedApiKey))
		{
			context.Response.StatusCode = 403; // Forbidden
			await context.Response.WriteAsync("Invalid API Key");
			return;
		}

		await _next(context);
	}
}