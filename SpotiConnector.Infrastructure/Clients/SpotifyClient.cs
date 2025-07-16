using Microsoft.Extensions.Options;
using SpotiConnector.Application.Interfaces;
using SpotiConnector.Infrastructure.DTO;
using SpotiConnector.Infrastructure.Options;
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

        public async Task<string> GetTokenAsync()
        {
            var body = new Dictionary<string, string>()
            {
                { "grant_type", "client_credentials" },
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Content = new FormUrlEncodedContent(body)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponseDTO>(json);

            return tokenResponse!.Access_token;
        }
    }
}
