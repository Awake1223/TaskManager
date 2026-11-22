using TaskManager.Application.DTOs.Common;
using TaskManager.Application.DTOs.Projects;

namespace TaskManager.Application.Abstractions.Services
{
    public interface IProjectService
    {
        Task<(ProjectDto? Dto, string? Error)> CreateAsync(CreateProjectRequest request, Guid currentUserId, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
        Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<ProjectDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
        Task<(ProjectDto? Dto, string? Error)> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken ct = default);
    }
}