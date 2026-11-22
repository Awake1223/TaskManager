using TaskManager.Domain.Models;

namespace TaskManager.Application.Abstractions.Repositories
{
    public interface IProjectRepository
    {
        Task AddAsync(ProjectModel project, CancellationToken ct = default);
        Task DeleteAsync(ProjectModel project, CancellationToken ct = default);
        Task<ProjectModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<(IReadOnlyList<ProjectModel> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
        Task UpdateAsync(ProjectModel project, CancellationToken ct = default);
    }
}