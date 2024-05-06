using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Firehose2.API_Server;
internal class APIServer
{
    public static void LaunchAPIServer()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(Program).Assembly.GetName().Name,
            ContentRootPath = Directory.GetCurrentDirectory(),
            WebRootPath = "wwwroot",
            EnvironmentName = Environments.Development
        });

        // Load the PFX certificate
        var certificate = new X509Certificate2("C:\\Users\\RARI\\Desktop\\Cert.pfx", "");

        IPAddress publicIPv4Address = IPAddress.Any;
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Listen(publicIPv4Address, 443, listenOptions =>
            {
                listenOptions.UseHttps(certificate);
            });
            serverOptions.Listen(publicIPv4Address, 80);  // Listen on port 80 for HTTP
        });

        // Add services to the container.
        builder.Services.AddControllers();

        // Configure CORS to allow any origin, method, and header.
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Add Swagger/OpenAPI support.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

        app.UseHttpsRedirection();

        app.UseAuthorization();

        // Apply the default CORS policy
        app.UseCors();

        // Map controllers
        app.MapControllers();

        // Redirect from the root URL to Swagger UI
        app.MapGet("/", context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });

        // Run the application
        app.Run();
    }
}
