namespace Roadmap.WeatherAPI.Configuration
{
    public record WeatherApiSettings
    {
        public required string ApiKey { get; init; }
        public required string BaseUrl { get; init; }
    }
}