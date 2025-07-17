using Microsoft.AspNetCore.Mvc;
using SpotiConnector.Application.Interfaces;
using SpotiConnector.Application.Services;

namespace SpotiConnector.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISpotifyAuthorizationService _spotifyAuthorizationService;
        public AuthController(ISpotifyAuthorizationService spotifyAuthorizationService)
        {
            _spotifyAuthorizationService = spotifyAuthorizationService;
        }

        [HttpGet("authorize")]
        public IActionResult AuthorizeUser()
        {
            var url = _spotifyAuthorizationService.GenerateAuthorizationUri();
            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> SpotifyCallback([FromQuery] string code, [FromQuery] string state)
        {
            // TODO: Validate state (security check)
            var redirectUri = "https://localhost:7001/api/auth/callback";
            var tokens = await _spotifyAuthorizationService.ExchangeUserCodeForAccessToken(code, redirectUri);
            // TODO: Store tokens
            return Ok(tokens);
        }
    }
}
