
using TaskManager.Application.Abstractions;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Domain.Models;


namespace TaskManager.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenGenerator _jwt;

        public AuthService(IUserRepository users, IJwtTokenGenerator jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        public async Task<(AuthResponse? Response, string? Error)> RegisterAsync(
            RegisterRequest request, CancellationToken ct = default)
        {
            var existing = await _users.GetByEmailAsync(request.Email, ct);
            if (existing is not null)
                return (null, "Пользователь с таким email уже существует.");

            var user = new UserModel
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _users.CreateAsync(user, request.Password, ct);
            if (!result.Succeeded)
                return (null, string.Join("; ", result.Errors.Select(e => e.Description)));

            var token = _jwt.GenerateToken(user.Id, user.Email!, $"{user.FirstName} {user.LastName}");
            return (new AuthResponse(token, user.Id, user.Email!, user.FirstName, user.LastName), null);
        }

        public async Task<(AuthResponse? Response, string? Error)> LoginAsync(
            LoginRequest request, CancellationToken ct = default)
        {
            var user = await _users.GetByEmailAsync(request.Email, ct);
            if (user is null || user.IsDeleted)
                return (null, "Неверный email или пароль.");

            var ok = await _users.CheckPasswordAsync(user, request.Password);
            if (!ok)
                return (null, "Неверный email или пароль.");

            var token = _jwt.GenerateToken(user.Id, user.Email!, $"{user.FirstName} {user.LastName}");
            return (new AuthResponse(token, user.Id, user.Email!, user.FirstName, user.LastName), null);
        }
    }
}
