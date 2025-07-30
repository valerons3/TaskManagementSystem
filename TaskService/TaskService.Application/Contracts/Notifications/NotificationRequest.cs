namespace TaskService.Application.Contracts.Notifications;

public record NotificationRequest(Guid UserId,
    string Title,
    string Message);