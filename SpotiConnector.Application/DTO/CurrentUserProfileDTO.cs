using System.Text.Json.Serialization;

namespace SpotiConnector.Application.DTO
{
    public class CurrentUserProfileDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
