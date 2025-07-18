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

        // <summary>
        // Generates authorization url for user to grant acces to their spotify account
        // </summary>
        // <returns>Redirects the user to Spotify's authorization endpoint.</returns>
        [HttpGet("authorize")]
        public IActionResult AuthorizeUser()
        {
            var url = _spotifyAuthorizationService.GenerateAuthorizationUri();
            return Redirect(url);
        }

        // <summary>
        // Catches spotify callback with user code which can be exchange for a token grating access for future calls (access level depends on scope sent on authorization)
        // </summary>
        // <returns>Returns spotify access token on successful user authorization</returns>
        [HttpGet("callback")]
        public async Task<IActionResult> SpotifyCallback([FromQuery] string code, [FromQuery] string state)
        {
            // TODO: Validate state (security check)
            var tokens = await _spotifyAuthorizationService.ExchangeUserCodeForAccessToken(code);
            // TODO: Store tokens
            return Ok(tokens);
        }
    }
}
