using System.Collections.Generic;
using dotnet_auth.Models;

namespace dotnet_auth.Services
{
  public interface IWeatherForecastService
  {
    IEnumerable<WeatherForecast> GetForecasts();
  }
}
