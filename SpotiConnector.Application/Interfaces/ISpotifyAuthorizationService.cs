using SpotiConnector.Application.DTO;

namespace SpotiConnector.Application.Interfaces
{
    public interface ISpotifyAuthorizationService
    {
        string GenerateAuthorizationUri();
        Task<AuthResultDTO> HandleSpotifyCallback(string code);
    }
}
