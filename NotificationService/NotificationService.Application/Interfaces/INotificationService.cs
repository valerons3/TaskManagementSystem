using NotificationService.Application.Contracts;

namespace NotificationService.Application.Interfaces;

public interface INotificationService
{
    Task CreateNotificationAsync(NotificationRequest request);
    Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId);
    Task MarkAsReadAsync(Guid id);
}