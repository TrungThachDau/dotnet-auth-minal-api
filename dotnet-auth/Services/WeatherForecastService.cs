using System;
using System.Collections.Generic;
using System.Linq;
using dotnet_auth.Models;

namespace dotnet_auth.Services
{
  public class WeatherForecastService : IWeatherForecastService
  {
    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    public IEnumerable<WeatherForecast> GetForecasts()
    {
      var rng = new Random();
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast(
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          rng.Next(-20, 55),
          Summaries[rng.Next(Summaries.Length)]
      ))
      .ToArray();
    }
  }
}
