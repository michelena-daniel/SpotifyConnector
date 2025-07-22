using Microsoft.Extensions.Options;
using SpotiConnector.Application.DTO;
using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Options;
using System.Net.Http.Headers;
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

        public async Task<SpotifyTokenResponseDTO?> GetUserTokenByAuthorizationCode(string code, string redirectUri)
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

            return tokenResponse;
        }

        public async Task<List<string>> GetUserTopTrackNamesAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/tracks");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(content);
            var tracks = json.RootElement.GetProperty("items");

            return [.. tracks.EnumerateArray().Select(track => track.GetProperty("name").GetString() ?? "Unknown Track")];
        }

        public async Task<CurrentUserProfileDTO?> GetCurrentUserProfileByAuthorizationCode(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<CurrentUserProfileDTO?>(content);

            return userResponse;
        }
    }
}
