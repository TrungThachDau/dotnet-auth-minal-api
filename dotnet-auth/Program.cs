using Microsoft.EntityFrameworkCore;
using dotnet_auth.Endpoints;
using dotnet_auth.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm cấu hình CORS cho phép frontend localhost:3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RDS")));
// Add OpenAPI services to the container
builder.Services.AddOpenApi();
// Add API explorer and Swagger generation services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register WeatherForecast service
builder.Services.AddScoped<IAuthService, AuthService>();


// Register the AppDbContext
builder.Services.AddScoped<AppDbContext>();

// Add authorization services
builder.Services.AddAuthorization();

builder.Services.AddAuthentication("JwtCookie")
    .AddCookie("JwtCookie", options =>
    {
        options.Cookie.Name = "jwt";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

// Build the application
var app = builder.Build();

// Sử dụng CORS policy cho toàn bộ app
app.UseCors("AllowFrontend");
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
app.UseAuthentication();
app.UseAuthorization();

// Map minimal API endpoints

app.MapAuthEndpoints();

// Run the application
app.Run();

