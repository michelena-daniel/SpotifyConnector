using Microsoft.Extensions.Caching.Distributed;
using SpotiConnector.Application.DTO;
using SpotiConnector.Application.Interfaces;
using System.Text.Json;

namespace SpotiConnector.Infrastructure.Cache
{
    public class SpotifyTokenCacheService : ISpotifyTokenCache
    {
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public SpotifyTokenCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        // TODO: create DTO to avoid tuple
        public async Task<(string userId, string AccessToken, string? RefreshToken)?> GetAsync(string userId)
        {
            var json = await _cache.GetAsync(userId);
            var dto = JsonSerializer.Deserialize<SpotifyTokenResponseDTO>(json);

            return(userId, dto.AccessToken, dto.RefreshToken);
        }

        public Task RemoveAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task StoreAsync(string userId, SpotifyTokenResponseDTO tokenResponse)
        {
            var payload = JsonSerializer.Serialize(tokenResponse);
            await _cache.SetStringAsync(userId, payload, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn) });
        }
    }
}
