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

        public async Task<SpotifyTokenResponseDTO?> HandleSpotifyCallback(string code)
        {
            // retrieve token and user
            var tokenResponse = await ExchangeUserCodeForAccessToken(code);
            if(tokenResponse == null)
                return null;
            var currentUser = await GetCurrentUserProfile(tokenResponse.AccessToken);
            if(currentUser == null)
                return null;
            // handle cache
            await _tokenCache.StoreAsync(currentUser.Id, tokenResponse);

            //  build JWT
            var claims = new[]
            {
                new Claim("spotify_id", currentUser.Id),
                new Claim(JwtRegisteredClaimNames.Sub, currentUser.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpMinutes),
                signingCredentials: creds);

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
