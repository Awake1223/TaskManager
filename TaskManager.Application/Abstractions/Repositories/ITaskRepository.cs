using TaskManager.Domain.Models;

namespace TaskManager.Application.Abstractions.Repositories
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItemModel task, CancellationToken ct = default);
        Task DeleteAsync(TaskItemModel task, CancellationToken ct = default);
        Task<TaskItemModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<(IReadOnlyList<TaskItemModel> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
        Task UpdateAsync(TaskItemModel task, CancellationToken ct = default);
    }
}