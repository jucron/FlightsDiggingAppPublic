using FlightsDiggingApp.Properties;
using FlightsDiggingApp.Services;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Populating Properties
builder.Configuration.AddJsonFile("amadeus_api_properties_values.json", optional: false, reloadOnChange: true);
builder.Services.Configure<AmadeusApiProperties>(builder.Configuration.GetSection("AmadeusApiValues"));

// Registering Dependency Injection
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IAuthService, AmadeusAuthService>();
builder.Services.AddSingleton<IApiService, AmadeusApiService>();
builder.Services.AddSingleton<IFlightsDiggerService, FlightsDiggerService>();
builder.Services.AddSingleton<IFilterService, FilterService>();

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

// Build App
var app = builder.Build();

// Dev tools
app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();              // Enable routing for controllers

//app.UseCors("AllowAngular"); // Apply CORS policy
app.UseCors("AllowAll");       // Apply CORS policy

app.UseWebSockets();           // Enable WebSocket support before routing

app.UseHttpsRedirection();     // Redirect HTTP to HTTPS (optional)

app.UseAuthorization();        // Optional: Apply Authorization (if necessary)

app.MapControllers();           // Map controllers

app.Run();
