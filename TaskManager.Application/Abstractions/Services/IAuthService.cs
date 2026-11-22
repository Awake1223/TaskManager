using TaskManager.Application.DTOs.Auth;

namespace TaskManager.Application.Abstractions.Services
{
    public interface IAuthService
    {
        Task<(AuthResponse? Response, string? Error)> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<(AuthResponse? Response, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    }
}