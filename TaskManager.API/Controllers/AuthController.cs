using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.DTOs.Auth;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
        {
            var (response, error) = await _auth.RegisterAsync(request, ct);
            if (response is null) return BadRequest(new { error });
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
        {
            var (response, error) = await _auth.LoginAsync(request, ct);
            if (response is null) return Unauthorized(new { error });
            return Ok(response);
        }
    }
}
