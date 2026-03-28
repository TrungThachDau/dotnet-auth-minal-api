﻿using dotnet_auth.Endpoints;

namespace dotnet_auth.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // ⚠️ Phải đặt đầu tiên để bắt tất cả exception
        app.UseExceptionHandler();

        // Use CORS
        app.UseCors("AllowFrontend");

        // Configure Swagger for all environments
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();

        // Configure development-only middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();

        return app;
    }
}
