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
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var url = $"{_settings.BaseUrl}?location={cityCode}&key={_settings.ApiKey}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Response content: {Content}", content);
            // Parse the Visual Crossing API response here
            try
            {
                var weatherData = JsonSerializer.Deserialize<WeatherResponse>(content, options);

                if (weatherData == null)
                {
                    _logger.LogError("Failed to deserialize weather response.");
                    throw new Exception("Error obtaining weather data.");
                }

                await _cacheService.SetAsync(cacheKey, weatherData);
                return weatherData;
            }
            catch (JsonException ex)
            {
                _logger.LogError("Error deserializando la respuesta: {ErrorMessage}", ex.Message);
                // Lanzar una excepción para que el llamador maneje el error
                throw new Exception("Error deserializando la respuesta del clima.", ex);
            }
        }
    }
}