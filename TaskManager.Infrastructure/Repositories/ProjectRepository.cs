

using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Models;
using TaskManager.Application.Abstractions.Repositories;

namespace TaskManager.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskManagerDbContext _context;

        public ProjectRepository(TaskManagerDbContext context) => _context = context;

        public Task<ProjectModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _context.Projects
                .Include(p => p.CreatedByUser)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<(IReadOnlyList<ProjectModel> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? search, CancellationToken ct = default)
        {
            var query = _context.Projects
                .AsNoTracking()
                .Include(p => p.CreatedByUser)
                .Include(p => p.Tasks)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(p => EF.Functions.ILike(p.Name, pattern));
            }

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task AddAsync(ProjectModel project, CancellationToken ct = default)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(ProjectModel project, CancellationToken ct = default)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(ProjectModel project, CancellationToken ct = default)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(ct);
        }
    }
}