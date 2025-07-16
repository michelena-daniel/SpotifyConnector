using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotiConnector.Infrastructure.DTO
{
    public class SpotifyTokenResponseDTO
    {
        [JsonPropertyName("access_token")]
        public string Access_token { get; set; } = null!;
        [JsonPropertyName("token_type")]
        public string Token_type { get; set; } = null!;
        [JsonPropertyName("expires_in")]
        public int Expires_in { get; set; }
    }
}
