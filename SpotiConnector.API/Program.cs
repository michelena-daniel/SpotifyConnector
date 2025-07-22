using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Options;
using SpotiConnector.Application.Services;
using SpotiConnector.Infrastructure.Cache;
using SpotiConnector.Infrastructure.Clients;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.Configure<SpotifyOptions>(builder.Configuration.GetSection("Spotify"));

builder.Services.AddHttpClient<ISpotifyClient, SpotifyClient>(client =>
{
    client.BaseAddress = new Uri("https://accounts.spotify.com/");
});
builder.Services.AddTransient<ISpotifyAuthorizationService, SpotifyAuthorizationService>();
builder.Services.AddScoped<ISpotifyTokenCache, SpotifyTokenCacheService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "spotifyConnector";
});

// Multiplexer functionality might not be needed yet, rolling iwth DistributedCache config for now
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var configuration = builder.Configuration.GetConnectionString("Redis");
//    return ConnectionMultiplexer.Connect(configuration!);
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();    
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
