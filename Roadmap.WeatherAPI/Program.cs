using Microsoft.AspNetCore.RateLimiting;
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
        builder.Services.Configure<WeatherApiSettings>(
            builder.Configuration.GetSection("WeatherApi"));
        builder.Services.Configure<RedisSettings>(
            builder.Configuration.GetSection("Redis"));

        //Register Redis cache service as singleton
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();

        //configure Rate Limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.PermitLimit = 10;
                opt.Window = TimeSpan.FromSeconds(10);
                opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 5;
            });
        });

        //Register Weather service
        builder.Services.AddSingleton<IWeatherService, WeatherService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRateLimiter();

        // Map endpoints
        app.MapWeatherEndpoints();

        app.Run();
    }
}