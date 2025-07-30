using Microsoft.AspNetCore.SignalR;
using NotificationService.API.Hubs;
using NotificationService.Application.Contracts;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Services;

public class NotificationHubClient : INotificationHubClient
{
    private readonly IHubContext<NotificationHub> hubContext;

    public NotificationHubClient(IHubContext<NotificationHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    public async Task SendNotificationAsync(NotificationRequest request)
    {
        await hubContext
            .Clients
            .Group(request.UserId.ToString())
            .SendAsync("ReceiveNotification", request.Title, request.Message);
    }
}