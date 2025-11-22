using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SpotiConnector.API.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly IHostEnvironment _env;

        public ErrorController(ILogger<ErrorController> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        [Route("/error")]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            if (exception == null)
            {
                return Problem(statusCode: 500, detail:
                    _env.IsDevelopment() ?
                    "Unknown error occurred." :
                    "An unexpected error occurred.");
            }

            int statusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ApplicationException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            var errorId = Guid.NewGuid().ToString();

            _logger.LogError(exception, "Unhandled exception occurred. ErrorId: {ErrorId}", errorId);

            var detail = _env.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred.";

            return Problem(
                detail: detail,
                statusCode: statusCode,
                extensions: new Dictionary<string, object?>
                {
                    ["errorId"] = errorId
                });
        }
    }
}
