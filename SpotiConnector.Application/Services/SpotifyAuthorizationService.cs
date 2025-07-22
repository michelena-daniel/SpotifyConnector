using Microsoft.Extensions.Options;
using SpotiConnector.Application.DTO;
using SpotiConnector.Application.Enums;
using SpotiConnector.Application.Extensions;
using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Options;
using System.Security.Cryptography;

namespace SpotiConnector.Application.Services
{
    public class SpotifyAuthorizationService : ISpotifyAuthorizationService
    {
        private readonly SpotifyOptions _options;
        private readonly ISpotifyClient _client;
        private ISpotifyTokenCache _tokenCache;

        public SpotifyAuthorizationService(IOptions<SpotifyOptions> options, ISpotifyClient client, ISpotifyTokenCache tokenCache) 
        {
            _options = options.Value;
            _client = client;
            _tokenCache = tokenCache;
        }

        public string GenerateAuthorizationUri()
        {
            return $"https://accounts.spotify.com/authorize" +
                   $"?client_id={_options.ClientId}" +
                   $"&response_type=code" +
                   $"&redirect_uri={_options.RedirectUri}" +
                   $"&scope={AuthorizationScopesEnum.UserReadCurrentlyPlaying.GetEnumMemberValue()+
                   " "+AuthorizationScopesEnum.UserTopRead.GetEnumMemberValue()}" +
                   $"&state={Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))}";
        }

        public async Task<SpotifyTokenResponseDTO> HandleSpotifyCallback(string code)
        {
            // HANDLE CALLBACK
            var tokenResponse = await ExchangeUserCodeForAccessToken(code);
            var currentUser = await GetCurrentUserProfile(tokenResponse.AccessToken);

            // HANDLE CACHE
            await _tokenCache.StoreAsync(currentUser.Id, tokenResponse);

            // RETURN DATA TO CONTROLLER
            return tokenResponse;

        }

        private async Task<SpotifyTokenResponseDTO?> ExchangeUserCodeForAccessToken(string code)
        {
            var accessTokenResponse = await _client.GetUserTokenByAuthorizationCode(code, _options.RedirectUri);

            return accessTokenResponse;
        }

        private async Task<CurrentUserProfileDTO?> GetCurrentUserProfile(string accessToken)
        {
            var currentUserProfile = await _client.GetCurrentUserProfileByAuthorizationCode(accessToken);

            return currentUserProfile;
        }
    }
}
