using Microsoft.AspNetCore.Identity;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<bool> CheckPasswordAsync(UserModel user, string password);
        Task<IdentityResult> CreateAsync(UserModel user, string password, CancellationToken ct = default);
        Task<UserModel?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<UserModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<(IReadOnlyList<UserModel> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
        Task UpdateAsync(UserModel user, CancellationToken ct = default);
    }
}