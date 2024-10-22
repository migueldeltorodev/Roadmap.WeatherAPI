using Microsoft.Extensions.Options;
using Roadmap.WeatherAPI.Configuration;
using Roadmap.WeatherAPI.Models;
using System.Text.Json;

namespace Roadmap.WeatherAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _settings;
        private readonly ICacheService _cacheService;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IOptions<WeatherApiSettings> settings,
        ICacheService cacheService, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<WeatherResponse> GetWeatherAsync(string cityCode, CancellationToken cancellationToken)
        {
            //redis key to store the cityCode value
            var cacheKey = $"weather:{cityCode}";

            // Try to get from cache first
            var cachedWeather = await _cacheService.GetAsync<WeatherResponse>(cacheKey);
            if (cachedWeather is not null)
            {
                _logger.LogInformation("Cache hit for city code: {CityCode}", cityCode);
                return cachedWeather;
            }

            _logger.LogInformation("Cache miss for city code: {CityCode}", cityCode);

            // If not in cache, call the API
            var url = $"{_settings.BaseUrl}?location={cityCode}&key={_settings.ApiKey}";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            // Parse the Visual Crossing API response here
            var weatherData = JsonSerializer.Deserialize<JsonDocument>(content);
            var weatherResponse = new WeatherResponse(
                CityCode: cityCode,
                Temperature: weatherData.RootElement.GetProperty("current").GetProperty("temperature").GetDouble(),
                Humidity: weatherData.RootElement.GetProperty("current").GetProperty("humidity").GetDouble(),
                Description: weatherData.RootElement.GetProperty("current").GetProperty("conditions").GetString()!,
                Timestamp: DateTime.UtcNow
            );

            // Cache the response
            await _cacheService.SetAsync(cacheKey, weatherResponse);

            return weatherResponse;
        }
    }
}