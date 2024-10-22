namespace Roadmap.WeatherAPI.Configuration
{
    public record RedisSettings
    {
        public required string ConnectionString { get; init; }
        public required int DatabaseId { get; init; }
        public required TimeSpan CacheExpiration { get; init; }
    }
}