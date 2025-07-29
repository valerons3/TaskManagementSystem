namespace NotificationService.Application.Contracts;

public record NotificationRequest(Guid UserId,
    string Title,
    string Message);