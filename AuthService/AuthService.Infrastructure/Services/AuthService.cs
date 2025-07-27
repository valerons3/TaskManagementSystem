using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Contracts.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly List<User> users = new List<User>();
    
    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        
        users.Add(user);
        
        var token = $"fake-jwt-token-for-{user.Username}";
        
        return Task.FromResult(new AuthResponse(user.Id, user.Username, user.Email, token));
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        User? user = users.FirstOrDefault(u =>
        u.Email == request.Email &&
            u.PasswordHash == HashPassword(request.Password));
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials");
        
        var token = $"fake-jwt-token-for-{user.Username}";

        return Task.FromResult(new AuthResponse(user.Id, user.Username, user.Email, token));
    }
    
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}