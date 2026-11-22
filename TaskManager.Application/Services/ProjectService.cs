using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.DTOs.Common;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projects;
        private readonly IUserRepository _users;

        public ProjectService(IProjectRepository projects, IUserRepository users)
        {
            _projects = projects;
            _users = users;
        }

        public async Task<PagedResult<ProjectDto>> GetPagedAsync(
            int page, int pageSize, string? search, CancellationToken ct = default)
        {
            (page, pageSize) = Normalize(page, pageSize);
            var (items, total) = await _projects.GetPagedAsync(page, pageSize, search, ct);
            return new PagedResult<ProjectDto>(items.Select(ToDto).ToList(), total, page, pageSize);
        }

        public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var project = await _projects.GetByIdAsync(id, ct);
            return project is null ? null : ToDto(project);
        }

        public async Task<(ProjectDto? Dto, string? Error)> CreateAsync(
            CreateProjectRequest request, Guid currentUserId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return (null, "Имя проекта обязательно.");

            var user = await _users.GetByIdAsync(currentUserId, ct);
            if (user is null || user.IsDeleted)
                return (null, "Текущий пользователь не найден.");

            var project = new ProjectModel
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description,
                CreatedByUserId = currentUserId,
                CreatedAt = DateTime.UtcNow,
                CreatedByUser = user
            };

            await _projects.AddAsync(project, ct);
            return (ToDto(project), null);
        }

        public async Task<(ProjectDto? Dto, string? Error)> UpdateAsync(
            Guid id, UpdateProjectRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return (null, "Имя проекта обязательно.");

            var project = await _projects.GetByIdAsync(id, ct);
            if (project is null)
                return (null, "Проект не найден.");

            project.Name = request.Name.Trim();
            project.Description = request.Description;
            project.UpdatedAt = DateTime.UtcNow;

            await _projects.UpdateAsync(project, ct);
            return (ToDto(project), null);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var project = await _projects.GetByIdAsync(id, ct);
            if (project is null) return false;

            await _projects.DeleteAsync(project, ct);
            return true;
        }

        private static ProjectDto ToDto(ProjectModel p) => new(
            p.Id,
            p.Name,
            p.Description,
            p.CreatedAt,
            p.CreatedByUserId,
            p.CreatedByUser is null ? string.Empty : $"{p.CreatedByUser.FirstName} {p.CreatedByUser.LastName}",
            p.Tasks?.Count ?? 0);

        private static (int Page, int PageSize) Normalize(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;
            return (page, pageSize);
        }
    }
}
