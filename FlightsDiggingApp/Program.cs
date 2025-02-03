using FlightsDiggingApp.Services;

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

// Registering Dependency Injection
builder.Services.AddSingleton<IApiService, ApiService>();
builder.Services.AddSingleton<IFlightsDiggerService, FlightsDiggerService>();

// Build App
var app = builder.Build();

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
