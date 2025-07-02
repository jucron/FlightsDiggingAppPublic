using FlightsDiggingApp.Properties;
using FlightsDiggingApp.Services;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using FlightsDiggingApp;

var builder = WebApplication.CreateBuilder(args);

BuilderHelper.AddControllers(builder);

BuilderHelper.AddSwagger(builder);

BuilderHelper.SetupCors(builder);

BuilderHelper.AddPropertiesDependencies(builder);

BuilderHelper.AddSingletonsDependencies(builder);

BuilderHelper.ConfigureLogger(builder);

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
