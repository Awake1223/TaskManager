
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.DTOs.Common;
using TaskManager.Application.DTOs.Users;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;

        public UserService(IUserRepository users) => _users = users;

        public async Task<PagedResult<UserDto>> GetPagedAsync(
            int page, int pageSize, string? search, CancellationToken ct = default)
        {
            (page, pageSize) = Normalize(page, pageSize);
            var (items, total) = await _users.GetPagedAsync(page, pageSize, search, ct);
            return new PagedResult<UserDto>(items.Select(ToDto).ToList(), total, page, pageSize);
        }

        public async Task<(UserDto? Dto, string? Error)> CreateAsync(
            CreateUserRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
                return (null, "Все поля обязательны.");

            var existing = await _users.GetByEmailAsync(request.Email, ct);
            if (existing is not null)
                return (null, "Пользователь с таким email уже существует.");

            var user = new UserModel
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            var result = await _users.CreateAsync(user, request.Password, ct);
            if (!result.Succeeded)
                return (null, string.Join("; ", result.Errors.Select(e => e.Description)));

            return (ToDto(user), null);
        }

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _users.GetByIdAsync(id, ct);
            if (user is null || user.IsDeleted) return false;

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _users.UpdateAsync(user, ct);
            return true;
        }

        private static UserDto ToDto(UserModel u) => new(
            u.Id, u.Email!, u.FirstName, u.LastName, u.IsDeleted, u.CreatedAt);

        private static (int Page, int PageSize) Normalize(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;
            return (page, pageSize);
        }
    }
}
