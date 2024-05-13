using System.Net;
using System.Security.Cryptography.X509Certificates;
using FirehoseServer.Summarisation;
using Hydrant;
using Microsoft.OpenApi.Models;

namespace FirehoseServer;
/* WAFFLE
 * Firehose 2.0
 * Firehose is a News Data Aggregator, this is a complete rewrite from python.
 * Firehose's mission is to try and get every news article in real time.
 *
 * History of Firehose
 * Dec 2023 - Firehose 1.0
 * Under IAI, the concept of firehose was created.
 * Here it was written in python and the concept of firehose was proven.
 *
 * Jan/Feb 2024 Firehose 1.5
 * After IAI disappeared due to *reasons* Firehose was tweaked to
 * better serve news analysis instead of its original purpose.
 * It was also rewritten in C# with a few tweaks to make it run better,
 * this had major performance improvements compared to 1.0
 *
 * April 2024 Firehose 2.0
 * Firehose 2.0 is another rewrite of firehose to make the codebase more
 * professional and make further use of multi-threading to make sure we are
 * using 100% of the CPU at all times.
 *
 * Future Ideas:
 *  - Publisher information scraping
 *  - Author information scraping
 *
 * May 2024 Firehose 3.0
 * Firehose 3.0 is a massive overhaul on how Firehose works:
 *  - Firehose now uses publication IDs to track publishers
 *  - HallonAPIServer is now merged into Firehose as a server thread
 *  - Add multiple endpoints for parsing stories at the same time
 *  - Switch to Gemma 1.1 it
 *
 * May 2024 -  BELLWETHER/3.1
 * Start to locate all references to companies.
 * Undo Hallon API Server (Raspberrys are now free again)
 *
 */
/// <summary>
/// Firehose, god's greatest news summarization engine.
///
/// V1 - Python (sucked)
/// V2 - C# Rewrite (sucked less)
/// V3 - Holy based
/// V4 - Incredibly based (YOU ARE HERE)
/// V5 - Summarises news so quick it attains a human form.
/// </summary>
public class Program
{
	/// <summary>
	/// Summary used to access database.
	/// </summary>
	internal static SQL MySQL = new("server=localhost;user=root;database=fhdb;port=3306;password=Mavik");
	public static List<HYDRANT.Definitions.Publication> Publications => HYDRANT.Definitions.Publication.LoadFromJSON();

	public static void Main(string[] args)
	{
		Task.Run(() => { new Thread(AISummaryController.Start).Start(); });

		Console.WriteLine("Firehose Initialised");

		var builder = WebApplication.CreateBuilder(new WebApplicationOptions
		{
			ApplicationName = typeof(Program).Assembly.GetName().Name,
			EnvironmentName = Environments.Production
		});

		builder.WebHost.ConfigureKestrel(serverOptions =>
		{
			serverOptions.Listen(IPAddress.Any, 5000, listenOptions =>
			{
				listenOptions.UseHttps(new X509Certificate2("C:\\Users\\RARI\\Desktop\\Cert.pfx"));
			});
		});
		// Add services to the container.
		builder.Services.AddControllers();

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "FIREHOSE SERVER", Version = "v4" });

			// Configure Swagger to use an API key
			c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Name = "ApiKey", // Name of the header to be used
				Type = SecuritySchemeType.ApiKey,
				Scheme = "ApiKeyScheme"
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "ApiKey"
						}
					},
					Array.Empty<string>()
				}
			});
		});
		var app = builder.Build();

		// Middleware to redirect the root URL to the Swagger UI
		app.UseSwagger();
		app.UseSwaggerUI();
		app.UseRouting();

		app.UseAuthorization();
		app.UseMiddleware<ApiKeyMiddleware>();
		app.UseMiddleware<RequestResponseLoggingMiddleware>();

		// Configure a redirection from the root ("/") to Swagger UI ("/swagger")
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapGet("/", context =>
			{
				context.Response.Redirect("/swagger");
				return Task.CompletedTask;
			});

			endpoints.MapControllers();
		});

		app.Run();
	}
}