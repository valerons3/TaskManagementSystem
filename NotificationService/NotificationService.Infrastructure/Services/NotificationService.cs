using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Contracts;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Persistence.DbContexts;

namespace NotificationService.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationDbContext dbContext;
    private readonly INotificationHubClient hubClient;

    public NotificationService(NotificationDbContext dbContext, INotificationHubClient hubClient)
    {
        this.dbContext = dbContext;
        this.hubClient = hubClient;
    }
    
    public async Task CreateNotificationAsync(NotificationRequest request)
    {
        Notification notification = new()
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Message = request.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
        
        await dbContext.Notifications.AddAsync(notification);
        await dbContext.SaveChangesAsync();
        await hubClient.SendNotificationAsync(request);
    }

    public async Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId)
    {
        return await dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationResponse(
                n.Id,
                n.Title,
                n.Message,
                n.IsRead,
                n.CreatedAt
            ))
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        Notification? notification = await dbContext.Notifications.FindAsync(id);
        if (notification is null)
            throw new Exception("Notification not found");

        notification.IsRead = true;
        await dbContext.SaveChangesAsync();
    }
}