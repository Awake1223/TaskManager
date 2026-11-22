using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;

        public UsersController(IUserService users) => _users = users;

        
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            CancellationToken ct = default)
            => Ok(await _users.GetPagedAsync(page, pageSize, search, ct));

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken ct)
        {
            var (dto, error) = await _users.CreateAsync(request, ct);
            return dto is null ? BadRequest(new { error }) : Ok(dto);
        }

        [HttpPost("{id:guid}/delete")]
        public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
        {
            var ok = await _users.SoftDeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
