using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext dbContext;

    public UserRepository(AuthDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    public async Task<bool> ExistsByIdentifierAsync(string email, string userName)
    {
        return await dbContext.Users.AnyAsync(u => u.Email == email || u.Username == userName);
    }

    public async Task<User?> GetByIdentifierAsync(string identifier)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == identifier || u.Username == identifier);
    }

    public async Task AddAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}