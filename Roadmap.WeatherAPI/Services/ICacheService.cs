namespace Roadmap.WeatherAPI.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value);

        Task<bool> DeleteAsync(string key);
    }
}