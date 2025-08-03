using Microsoft.EntityFrameworkCore;
using dotnet_auth.Endpoints;
using dotnet_auth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add OpenAPI services to the container
builder.Services.AddOpenApi();
// Add API explorer and Swagger generation services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register WeatherForecast service
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

// Build the application
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    // Map OpenAPI endpoints in development environment
    app.MapOpenApi();
}
if (app.Environment.IsDevelopment())
{
    // Enable Swagger and Swagger UI in development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Map minimal API endpoints
app.MapWeatherForecastEndpoints();


app.MapAuthEndpoints();

// Run the application
app.Run();

