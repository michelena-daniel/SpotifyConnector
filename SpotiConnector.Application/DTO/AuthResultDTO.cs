namespace SpotiConnector.Application.DTO
{
    public class AuthResultDTO
    {
        public string? JwtToken { get; set; }
        public SpotifyTokenResponseDTO? SpotifyToken { get; set; }
    }
}
