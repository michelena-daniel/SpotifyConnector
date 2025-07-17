using Microsoft.Extensions.Options;
using SpotiConnector.Application.DTO;
using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Options;
using System.Text.Json;

namespace SpotiConnector.Infrastructure.Clients
{
    public class SpotifyClient : ISpotifyClient
    {
        private HttpClient _httpClient { get; set; }
        private SpotifyOptions _options { get; set; }

        public SpotifyClient(HttpClient httpClient, IOptions<SpotifyOptions> options) 
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> GetUserTokenByAuthorizationCode(string code, string redirectUri)
        {
            var body = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri }
            };

            var credentials = $"{_options.ClientId}:{_options.ClientSecret}";
            var encodedCredentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Content = new FormUrlEncodedContent(body)
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encodedCredentials);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponseDTO>(json);

            return tokenResponse!.AccessToken;
        }
    }
}
