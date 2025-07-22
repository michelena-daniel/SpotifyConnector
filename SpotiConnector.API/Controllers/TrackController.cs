using Microsoft.AspNetCore.Mvc;
using SpotiConnector.Application.Interfaces;

namespace SpotiConnector.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class TrackController : ControllerBase
    {
        private readonly ISpotifyClient _spotifyClient;
        private ISpotifyTokenCache _tokenCache;
        public TrackController(ISpotifyClient spotifyClient, ISpotifyTokenCache spotifyTokenCache) 
        {
            _spotifyClient = spotifyClient;
            _tokenCache = spotifyTokenCache;
        }

        [HttpGet("top-user-tracks")]
        public async Task<IActionResult> GetUserTopTracks()
        {
            //TODO: ADD JWT authorization carrying the spotify token
            // get token
            //_tokenCache.GetAsync();

            //get tracks
            var trackNames = await _spotifyClient.GetUserTopTrackNamesAsync("");
            return Ok(trackNames);
        }
    }
}
