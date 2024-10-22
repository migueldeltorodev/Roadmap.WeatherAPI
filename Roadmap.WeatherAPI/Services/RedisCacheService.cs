using Microsoft.Extensions.Options;
using Roadmap.WeatherAPI.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace Roadmap.WeatherAPI.Services
{
    public sealed class RedisCacheService : ICacheService, IDisposable
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly TimeSpan _expiration;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IOptions<RedisSettings> settings, ILogger<RedisCacheService> logger)
        {
            _redis = ConnectionMultiplexer.Connect(settings.Value.ConnectionString);
            _db = _redis.GetDatabase(settings.Value.DatabaseId);
            _expiration = settings.Value.CacheExpiration;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                if (value.IsNull)
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving value from Redis for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                await _db.StringSetAsync(key, serializedValue, _expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in Redis for key: {Key}", key);
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting key from Redis: {Key}", key);
                return false;
            }
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}