using Microsoft.IdentityModel.Tokens;
using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Options;
using SpotiConnector.Application.Services;
using SpotiConnector.Infrastructure.Cache;
using SpotiConnector.Infrastructure.Clients;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.Configure<SpotifyOptions>(builder.Configuration.GetSection("Spotify"));
builder.Services.AddHttpClient<ISpotifyClient, SpotifyClient>(client =>
{
    client.BaseAddress = new Uri("https://accounts.spotify.com/");
});
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt!.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey
                = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(jwt.SigningKey))

        };
    });
builder.Services.AddAuthorization();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "spotifyConnector";
});
builder.Services.AddTransient<ISpotifyAuthorizationService, SpotifyAuthorizationService>();
builder.Services.AddScoped<ISpotifyTokenCache, SpotifyTokenCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{   
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler("/error");
app.MapControllers();

app.Run();
