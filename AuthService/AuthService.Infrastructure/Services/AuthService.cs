using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Contracts.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext dbContext;
    private readonly IJwtTokenGenerator tokenGenerator;

    public AuthService(AuthDbContext dbContext, IJwtTokenGenerator tokenGenerator)
    {
        this.dbContext = dbContext;
        this.tokenGenerator = tokenGenerator;
    }
    
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await dbContext.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            throw new ApplicationException($"User with email {request.Email} already exists");
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        var token = tokenGenerator.GenerateToken(user);
        return new AuthResponse(user.Id, user.Username, user.Email, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        User? user = await dbContext.Users.FirstOrDefaultAsync(u =>
            u.Email == request.Email && 
            u.PasswordHash == HashPassword(request.Password));
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials");
        
        var token = tokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Username, user.Email, token);
    }
    
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}