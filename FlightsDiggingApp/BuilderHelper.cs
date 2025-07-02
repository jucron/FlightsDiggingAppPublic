

using FlightsDiggingApp.Properties;
using FlightsDiggingApp.Services;
using Microsoft.Extensions.Logging.Console;

namespace FlightsDiggingApp
{
    public class BuilderHelper
    {
        internal static void AddControllers(WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllers();
        }
        internal static void AddPropertiesDependencies(WebApplicationBuilder builder)
        {
            // Populating Properties
            builder.Configuration.AddJsonFile("amadeus_api_properties_values.json", optional: false, reloadOnChange: true);
            builder.Services.Configure<AmadeusApiProperties>(builder.Configuration.GetSection("AmadeusApiValues"));
        }

        internal static void AddSingletonsDependencies(WebApplicationBuilder builder)
        {
            // Registering Dependency Injection
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddSingleton<IAuthService, AmadeusAuthService>();
            builder.Services.AddSingleton<IApiService, AmadeusApiService>();
            builder.Services.AddSingleton<IFlightsDiggerService, FlightsDiggerService>();
            builder.Services.AddSingleton<IFilterService, FilterService>();
        }

        internal static void AddSwagger(WebApplicationBuilder builder)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        internal static void ConfigureLogger(WebApplicationBuilder builder)
        {
            // Configure logging using ConsoleFormatterOptions
            builder.Logging.ClearProviders(); // Remove default providers
            builder.Logging.AddConsole(options =>
            {
                options.FormatterName = ConsoleFormatterNames.Simple; // Use "Simple" format
            });

            builder.Services.Configure<SimpleConsoleFormatterOptions>(options =>
            {
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss "; // Set timestamp format
                options.IncludeScopes = true; // Optional: Show log scopes
            });
        }

        internal static void SetupCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    // Allow any origin (less secure, use only in development):
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Replace with your Angular app URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }
    }
}
