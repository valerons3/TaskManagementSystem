using NotificationService.Application.Contracts;
using NotificationService.Application.Exceptions;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository repository;
    private readonly INotificationHubClient hubClient;

    public NotificationService(INotificationRepository repository, INotificationHubClient hubClient)
    {
        this.repository = repository;
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
        
        await repository.AddAsync(notification);
        await repository.SaveChangesAsync();
        await hubClient.SendNotificationAsync(request);
    }

    public async Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId)
    {
        var entities = await repository.GetByUserIdAsync(userId);
    
        return entities.Select(n => new NotificationResponse(
            n.Id,
            n.Title,
            n.Message,
            n.IsRead,
            n.CreatedAt
        ));
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        Notification? notification = await repository.GetByIdAsync(id);
        if (notification is null)
            throw new NotFoundException($"Notification with id: {id} not found");

        notification.IsRead = true;
        await repository.SaveChangesAsync();
    }
}