using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Contracts.Auth;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext dbContext;
    private readonly IJwtTokenGenerator tokenGenerator;
    private readonly IPasswordHasher passwordHasher;

    public AuthService(AuthDbContext dbContext, IJwtTokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
    {
        this.dbContext = dbContext;
        this.tokenGenerator = tokenGenerator;
        this.passwordHasher = passwordHasher;
    }
    
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await dbContext.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            throw new UserAlreadyExistsException(request.Email);
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        var token = tokenGenerator.GenerateToken(user);
        return new AuthResponse(user.Id, user.Username, user.Email, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();
        
        var token = tokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Username, user.Email, token);
    }
}