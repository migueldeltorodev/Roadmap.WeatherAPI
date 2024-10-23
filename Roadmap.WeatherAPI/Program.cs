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

        //configure services from appsettings.json
        builder.Services.Configure<WeatherApiSettings>(
            builder.Configuration.GetSection("WeatherApi"));
        builder.Services.Configure<RedisSettings>(
            builder.Configuration.GetSection("Redis"));

        //Register Redis cache service as singleton
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();

        //configure Rate Limiting middleware
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
        builder.Services.AddHttpClient<IWeatherService, WeatherService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRateLimiter();

        app.MapGet("/weather/{cityCode}", async (string cityCode, IWeatherService weatherService) =>
        {
            try
            {
                var weatherResponse = await weatherService.GetWeatherAsync(cityCode, CancellationToken.None);
                return Results.Ok(weatherResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el clima: {ex.Message}");
                return Results.BadRequest(ex.Message);
            }
        });

        app.Run();
    }
}