namespace Roadmap.WeatherAPI.Models
{
    public record WeatherResponse(
        string CityCode,
        double Temperature,
        double Humidity,
        string Description,
        DateTime Timestamp);
}