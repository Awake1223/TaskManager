using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.DTOs.Common;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _tasks;
        private readonly IProjectRepository _projects;
        private readonly IUserRepository _users;

        public TaskService(ITaskRepository tasks, IProjectRepository projects, IUserRepository users)
        {
            _tasks = tasks;
            _projects = projects;
            _users = users;
        }

        public async Task<PagedResult<TaskDto>> GetPagedAsync(
            int page, int pageSize, string? search, CancellationToken ct = default)
        {
            (page, pageSize) = Normalize(page, pageSize);
            var (items, total) = await _tasks.GetPagedAsync(page, pageSize, search, ct);
            return new PagedResult<TaskDto>(items.Select(ToDto).ToList(), total, page, pageSize);
        }

        public async Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var task = await _tasks.GetByIdAsync(id, ct);
            return task is null ? null : ToDto(task);
        }

        public async Task<(TaskDto? Dto, string? Error)> CreateAsync(
            CreateTaskRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return (null, "Имя задачи обязательно.");

            var project = await _projects.GetByIdAsync(request.ProjectId, ct);
            if (project is null)
                return (null, "Проект не найден.");

            if (request.AssigneeId.HasValue)
            {
                var assignee = await _users.GetByIdAsync(request.AssigneeId.Value, ct);
                if (assignee is null || assignee.IsDeleted)
                    return (null, "Исполнитель не найден.");
            }

            var task = new TaskItemModel
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description,
                ProjectId = request.ProjectId,
                AssigneeId = request.AssigneeId,
                CreatedAt = DateTime.UtcNow
            };

            await _tasks.AddAsync(task, ct);

            task.Project = project;
            return (ToDto(task), null);
        }

        public async Task<(TaskDto? Dto, string? Error)> UpdateAsync(
            Guid id, UpdateTaskRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return (null, "Имя задачи обязательно.");

            var task = await _tasks.GetByIdAsync(id, ct);
            if (task is null)
                return (null, "Задача не найдена.");

            if (request.AssigneeId.HasValue)
            {
                var assignee = await _users.GetByIdAsync(request.AssigneeId.Value, ct);
                if (assignee is null || assignee.IsDeleted)
                    return (null, "Исполнитель не найден.");
            }

            task.Name = request.Name.Trim();
            task.Description = request.Description;
            task.AssigneeId = request.AssigneeId;
            task.UpdatedAt = DateTime.UtcNow;

            await _tasks.UpdateAsync(task, ct);


            var refreshed = await _tasks.GetByIdAsync(id, ct) ?? task;
            return (ToDto(refreshed), null);
        }

        public async Task<(TaskDto? Dto, string? Error)> UpdateStatusAsync(
            Guid id, UpdateTaskStatusRequest request, CancellationToken ct = default)
        {
            var task = await _tasks.GetByIdAsync(id, ct);
            if (task is null)
                return (null, "Задача не найдена.");

            task.Status = request.Status;
            task.UpdatedAt = DateTime.UtcNow;

            await _tasks.UpdateAsync(task, ct);
            return (ToDto(task), null);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var task = await _tasks.GetByIdAsync(id, ct);
            if (task is null) return false;

            await _tasks.DeleteAsync(task, ct);
            return true;
        }

        private static TaskDto ToDto(TaskItemModel t) => new(
            t.Id,
            t.Name,
            t.Description,
            t.Status,
            t.ProjectId,
            t.Project?.Name ?? string.Empty,
            t.AssigneeId,
            t.Assignee is null ? null : $"{t.Assignee.FirstName} {t.Assignee.LastName}",
            t.CreatedAt,
            t.UpdatedAt);

        private static (int Page, int PageSize) Normalize(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;
            return (page, pageSize);
        }
    }
}
