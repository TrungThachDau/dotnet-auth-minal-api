using dotnet_auth.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services
    .AddCorsConfiguration(builder.Configuration)
    .AddDatabaseConfiguration(builder.Configuration)
    .AddJwtConfiguration(builder.Configuration)
    .AddApplicationServices()
    .AddErrorHandling()
    .AddSwaggerConfiguration();

var app = builder.Build();

// Configure middleware pipeline
app.ConfigureMiddleware()
    .MapEndpoints();

app.Run();