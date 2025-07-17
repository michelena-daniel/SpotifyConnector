using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotiConnector.Application.DTO
{
    public class SpotifyTokenResponseDTO
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = null!;
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = null!;
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = null!;
    }
}
