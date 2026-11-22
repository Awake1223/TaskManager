using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _tasks;

        public TasksController(ITaskService tasks) => _tasks = tasks;

        /// <summary>Постраничный список задач с поиском по имени.</summary>
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            CancellationToken ct = default)
            => Ok(await _tasks.GetPagedAsync(page, pageSize, search, ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _tasks.GetByIdAsync(id, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskRequest request, CancellationToken ct)
        {
            var (dto, error) = await _tasks.CreateAsync(request, ct);
            return dto is null
                ? BadRequest(new { error })
                : CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request, CancellationToken ct)
        {
            var (dto, error) = await _tasks.UpdateAsync(id, request, ct);
            return dto is null ? BadRequest(new { error }) : Ok(dto);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusRequest request, CancellationToken ct)
        {
            var (dto, error) = await _tasks.UpdateStatusAsync(id, request, ct);
            return dto is null ? BadRequest(new { error }) : Ok(dto);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var ok = await _tasks.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
