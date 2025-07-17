using Microsoft.Extensions.Options;
using SpotiConnector.Application.DTO;
using SpotiConnector.Application.Enums;
using SpotiConnector.Application.Extensions;
using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpotiConnector.Application.Services
{
    public class SpotifyAuthorizationService : ISpotifyAuthorizationService
    {
        private SpotifyOptions _options;
        private ISpotifyClient _client;
        public SpotifyAuthorizationService(IOptions<SpotifyOptions> options, ISpotifyClient client) 
        {
            _options = options.Value;
            _client = client;
        }

        public string GenerateAuthorizationUri()
        {
            var redirectUri = "https://localhost:7001/api/auth/callback";
            return $"https://accounts.spotify.com/authorize" +
                   $"?client_id={_options.ClientId}" +
                   $"&response_type=code" +
                   $"&redirect_uri={redirectUri}" +
                   $"&scope={AuthorizationScopesEnum.UserReadCurrentlyPlaying.GetEnumMemberValue()}" +
                   $"&state={Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))}";
        }

        public async Task<string> ExchangeUserCodeForAccessToken(string code, string redirectUri)
        {
            var accessToken = await _client.GetUserTokenByAuthorizationCode(code, redirectUri);

            return accessToken;
        }
    }
}
