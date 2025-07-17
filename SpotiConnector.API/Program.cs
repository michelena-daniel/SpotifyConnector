using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Options;
using SpotiConnector.Application.Services;
using SpotiConnector.Infrastructure.Clients;

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
