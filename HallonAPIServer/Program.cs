using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace HallonAPIServer;

public class Program
{
    public static List<HYDRANT.Publication> Publications => HYDRANT.Publication.LoadFromJSON("..\\FirehoseServer\\Data\\Feeds.json");

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(Program).Assembly.GetName().Name,
            EnvironmentName = Environments.Production
        });

        var certificate = new X509Certificate2("C:\\Users\\RARI\\Desktop\\Cert.pfx", "");

        IPAddress publicIPv4Address = IPAddress.Any;
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Listen(publicIPv4Address, 5000, listenOptions =>
            {
                listenOptions.UseHttps(certificate);
            });
            serverOptions.Listen(publicIPv4Address, 443, listenOptions =>
            {
                listenOptions.UseHttps(certificate);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
            });
            serverOptions.Listen(publicIPv4Address, 80);  // Listen on port 80 for HTTP
        });
        // Add services to the container.
        builder.Services.AddControllers();

        //Kindly ask CORS to go away
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        // Use CORS with the default policy
        app.UseCors();

        app.MapControllers();

        app.Run();
    }
}
