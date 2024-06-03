using System.Management;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using FirehoseServer.Summarisation;
using Hydrant;
using Microsoft.OpenApi.Models;

namespace FirehoseServer;
/* FIREHOSE LATEST REQUIREMENTS
 * FirehoseServer is an open source tool.
 * I guarantee no compatibility with systems other than my own.
 * I've tried to ensure at every point it can work on systems other than
 * my own, but I cannot foresee every possible combination of hardware.
 *
 * As such, to get Firehose Server working on your own machine you may need
 * to make modifications to it. You are encouraged to contribute these back to
 * the Firehose Repo to help others summarise news.
 *
 * To help you roughly understand what's required, here's my dev machine
 * AMD Ryzen 7 5800x
 * NVIDIA RTX 3090
 * 48GB DDR4 3200mhz
 * 3TB NVME RAID
 * Windows 11 24H1
 *
 * This is obviously overkill, I imagine you can get similar levels of
 * performance with much less but obviously unless you are using the exact
 * same config above I can't help you.
 *
 * You'll also need the following software
 * MySQL, BigShorts/MarketInfo, LM Studio
 * Currently we use Gemma 2b and Gemma 7b as they are about good enough for now
 * as they are fast and make ok-ish summaries. The model can and will change
 * on a whim as I aim to strike a balance between fast and accurate summaries.
 * Mistral7B is good but slow, perhaps llama3 or the next ver of Gemma.
 *
 */
/* Version History
 * NOTE: Firehose versions pre 3.2 are NOT publicly available.
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
 * May 2024 - Firehose 3.1
 * Start to locate all references to companies.
 * Undo Hallon API Server (Raspberrys are now free again)
 * unundo hallon api server merge (Raspberrys are now subjugated.)
 *
 * June 2024 - Firehose 3.2
 *  - Replace TNS implementation with TheBigShorts ver
 *  - Fixed MySQL vulnerability
 *  - Check and fix any potential issues with the setup.
 */
public class Program
{
	/// <summary>
	/// Summary used with write-access database.
	/// </summary>
	internal static SQL MySQLAdmin = new("server=localhost;user=root;database=fhdb;port=3306;password=Mavik");
    
    /// <summary>
    /// Read Only SQL Access
    /// </summary>
    internal static SQL MySQLReadOnly = new("server=localhost;user=HALLON;database=fhdb;port=3306;password=HALLON");
	public static List<HYDRANT.Definitions.Publication> Publications => HYDRANT.Definitions.Publication.LoadFromJSON();

	public static void Main(string[] args)
    {
        Precheck();

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

    /// <summary>
    /// Before we launch firehose we want to
    /// </summary>
    public static void Precheck()
    {
        Console.WriteLine("Checking GPU...");
        bool GPU;
        try
        {
            using (ManagementObjectSearcher searcher = new("SELECT * FROM Win32_VideoController"))
            {
                GPU = searcher.Get()
                    .Cast<ManagementObject>()
                    .Select(obj => obj["Name"]?.ToString())
                    .Where(name => name != null)
                    .ToList().Any(name => name.Contains("NVIDIA") && name.Contains("RTX"));
            }
            Console.WriteLine($"GPU: {(GPU ? "RTX GPU Found" : "RTX GPU not found.")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking GPU: {ex.Message}");
        }
    }

}
