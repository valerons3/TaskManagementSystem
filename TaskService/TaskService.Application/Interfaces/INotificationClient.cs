using TaskService.Application.Contracts.Notifications;

namespace TaskService.Application.Interfaces;

public interface INotificationClient
{
    Task SendNotificationAsync(NotificationRequest request);
}