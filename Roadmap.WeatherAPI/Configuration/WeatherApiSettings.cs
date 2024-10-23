namespace Roadmap.WeatherAPI.Configuration
{
    public record WeatherApiSettings
    {
        public required string BaseUrl { get; init; }
    }
}