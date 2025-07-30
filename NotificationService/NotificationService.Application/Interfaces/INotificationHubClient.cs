using NotificationService.Application.Contracts;

namespace NotificationService.Application.Interfaces;

public interface INotificationHubClient
{
    Task SendNotificationAsync(NotificationRequest request);
}