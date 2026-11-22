using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Models;
using TaskManager.Application.Abstractions.Repositories;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagerDbContext _context;

    public TaskRepository(TaskManagerDbContext context) => _context = context;

    public Task<TaskItemModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<(IReadOnlyList<TaskItemModel> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(t => EF.Functions.ILike(t.Name, pattern));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(TaskItemModel task, CancellationToken ct = default)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TaskItemModel task, CancellationToken ct = default)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TaskItemModel task, CancellationToken ct = default)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(ct);
    }
}