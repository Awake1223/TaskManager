using TaskManager.Application.DTOs.Common;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<(UserDto? Dto, string? Error)> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
        Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default);
    }
}