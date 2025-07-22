using SpotiConnector.Application.DTO;

namespace SpotiConnector.Application.Interfaces
{
    public interface ISpotifyClient
    {
        Task<SpotifyTokenResponseDTO?> GetUserTokenByAuthorizationCode(string code, string redirectUri);
        Task<CurrentUserProfileDTO?> GetCurrentUserProfileByAuthorizationCode(string accesToken);
        Task<List<string>> GetUserTopTrackNamesAsync(string accessToken);
    }
}
