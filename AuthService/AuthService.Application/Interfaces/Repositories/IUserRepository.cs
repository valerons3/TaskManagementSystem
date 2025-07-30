using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}