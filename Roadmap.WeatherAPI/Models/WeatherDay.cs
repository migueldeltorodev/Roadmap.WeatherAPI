namespace Roadmap.WeatherAPI.Models
{
    public class WeatherDay
    {
        public DateTime Datetime { get; set; }
        public double TempMax { get; set; }
        public double TempMin { get; set; }
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public double Humidity { get; set; }
        public string Conditions { get; set; }
    }
}