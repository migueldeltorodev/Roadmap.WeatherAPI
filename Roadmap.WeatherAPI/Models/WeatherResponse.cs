namespace Roadmap.WeatherAPI.Models
{
    public class WeatherResponse
    {
        public int QueryCost { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ResolvedAddress { get; set; }
        public string Address { get; set; }
        public string TimeZone { get; set; }
        public List<WeatherDay> Days { get; set; }
    }
}