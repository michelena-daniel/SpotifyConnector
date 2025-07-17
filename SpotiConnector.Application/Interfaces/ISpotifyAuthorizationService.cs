namespace SpotiConnector.Application.Interfaces
{
    public interface ISpotifyAuthorizationService
    {
        string GenerateAuthorizationUri();
        Task<string> ExchangeUserCodeForAccessToken(string code, string redirectUri);
    }
}
