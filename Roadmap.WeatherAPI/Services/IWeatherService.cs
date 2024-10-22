using Roadmap.WeatherAPI.Models;

namespace Roadmap.WeatherAPI.Services
{
    public interface IWeatherService
    {
        Task<WeatherResponse> GetWeatherAsync(string cityCode, CancellationToken cancellationToken);
    }
}