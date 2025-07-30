using AuthService.Application.Contracts.Auth;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IJwtTokenGenerator tokenGenerator;
    private readonly IPasswordHasher passwordHasher;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
    {
        this.userRepository = userRepository;
        this.tokenGenerator = tokenGenerator;
        this.passwordHasher = passwordHasher;
    }
    
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await userRepository.ExistsByEmailAsync(request.Email);
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
        
        await userRepository.AddAsync(user);
        
        var token = tokenGenerator.GenerateToken(user);
        return new AuthResponse(user.Id, user.Username, user.Email, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();
        
        var token = tokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Username, user.Email, token);
    }
}