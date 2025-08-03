using Microsoft.AspNetCore.Builder;
using System;
using dotnet_auth.Services;

namespace dotnet_auth.Endpoints
{
  public static class WeatherForecastEndpoints
  {
    public static void MapWeatherForecastEndpoints(this WebApplication app)
    {
      app.MapGet("/weatherforecast", (IWeatherForecastService svc) => svc.GetForecasts())
         .WithName("GetWeatherForecast");
    }

  }
}
