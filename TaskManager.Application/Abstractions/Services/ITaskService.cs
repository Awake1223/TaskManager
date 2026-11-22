using TaskManager.Application.DTOs.Common;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Abstractions.Services
{
    public interface ITaskService
    {
        Task<(TaskDto? Dto, string? Error)> CreateAsync(CreateTaskRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
        Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<TaskDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
        Task<(TaskDto? Dto, string? Error)> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default);
        Task<(TaskDto? Dto, string? Error)> UpdateStatusAsync(Guid id, UpdateTaskStatusRequest request, CancellationToken ct = default);
    }
}