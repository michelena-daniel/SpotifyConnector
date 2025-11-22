using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpotiConnector.Application.DTO;
using SpotiConnector.Application.Enums;
using SpotiConnector.Application.Extensions;
using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SpotiConnector.Application.Services
{
    public class SpotifyAuthorizationService : ISpotifyAuthorizationService
    {
        private readonly SpotifyOptions _options;
        private readonly JwtOptions _jwtOptions;
        private readonly ISpotifyClient _client;
        private readonly ISpotifyTokenCache _tokenCache;

        public SpotifyAuthorizationService(IOptions<SpotifyOptions> options, ISpotifyClient client, ISpotifyTokenCache tokenCache, IOptions<JwtOptions> jwtOptions) 
        {
            _options = options.Value;
            _client = client;
            _tokenCache = tokenCache;
            _jwtOptions = jwtOptions.Value;
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

        public async Task<AuthResultDTO?> HandleSpotifyCallback(string code)
        {
            SpotifyTokenResponseDTO? tokenResponse = await ExchangeUserCodeForAccessToken(code) 
                ?? throw new ApplicationException("Failed to exchange authorization code for token.");
            CurrentUserProfileDTO? currentUser = await GetCurrentUserProfile(tokenResponse.AccessToken) 
                ?? throw new ApplicationException("Failed to retrieve Spotify user profile.");
            await _tokenCache.StoreAsync(currentUser.Id, tokenResponse);
            string jwtToken = BuildJwt(currentUser.Id);

            return new AuthResultDTO
            {
                JwtToken = jwtToken,
                SpotifyToken = tokenResponse
            };
        }

        private string BuildJwt(string spotifyUserId)
        {
            Claim[] claims =
            [
                new Claim("spotify_id", spotifyUserId),
                new Claim(JwtRegisteredClaimNames.Sub, spotifyUserId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];
            SymmetricSecurityKey? key = new(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            SigningCredentials? creds = new(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwt = new(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpMinutes),
                signingCredentials: creds);
            JwtSecurityTokenHandler jwtHandler = new();
            return jwtHandler.WriteToken(jwt);
        }

        private async Task<SpotifyTokenResponseDTO?> ExchangeUserCodeForAccessToken(string code)
        {
            SpotifyTokenResponseDTO? accessTokenResponse = await _client.GetUserTokenByAuthorizationCode(code, _options.RedirectUri);

            return accessTokenResponse;
        }

        private async Task<CurrentUserProfileDTO?> GetCurrentUserProfile(string accessToken)
        {
            CurrentUserProfileDTO? currentUserProfile = await _client.GetCurrentUserProfileByAuthorizationCode(accessToken);

            return currentUserProfile;
        }
    }
}
