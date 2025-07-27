using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}