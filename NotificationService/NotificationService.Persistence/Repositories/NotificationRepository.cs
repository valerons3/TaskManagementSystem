using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Domain.Entities;
using NotificationService.Persistence.DbContexts;

namespace NotificationService.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext dbContext;
        
    public NotificationRepository(NotificationDbContext context)
    {
        dbContext = context;
    }
    
    public async Task AddAsync(Notification notification)
    {
        await dbContext.Notifications.AddAsync(notification);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
    {
        return await dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await dbContext.Notifications.FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}