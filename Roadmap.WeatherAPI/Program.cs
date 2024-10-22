using Roadmap.WeatherAPI.Configuration;
using Roadmap.WeatherAPI.Services;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //add services
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //configure services
        builder.Services.Configure<RedisSettings>(
            builder.Configuration.GetSection("Redis"));

        //Register Redis cache service as singleton
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();

        var app = builder.Build();

        app.Run();
    }
}