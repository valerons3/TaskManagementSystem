using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByIdentifierAsync(string email, string userName);
    Task<User?> GetByIdentifierAsync(string identifier);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}