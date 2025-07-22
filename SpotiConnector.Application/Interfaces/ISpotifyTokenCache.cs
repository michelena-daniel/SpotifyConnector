using SpotiConnector.Application.DTO;

namespace SpotiConnector.Application.Interfaces
{
    public interface ISpotifyTokenCache
    {
        Task StoreAsync(string userId, SpotifyTokenResponseDTO tokenResponse);
        Task<(string userId, string AccessToken, string? RefreshToken)?> GetAsync(string userId);
        Task RemoveAsync(string userId);
    }
}
