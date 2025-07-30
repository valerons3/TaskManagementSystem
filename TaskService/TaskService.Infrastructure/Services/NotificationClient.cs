using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using TaskService.Application.Contracts.Notifications;
using TaskService.Application.Exceptions;
using TaskService.Application.Interfaces;

namespace TaskService.Infrastructure.Services;

public class NotificationClient : INotificationClient
{
    private readonly HttpClient client;
    private readonly string notificationsEndpoint;

    public NotificationClient(HttpClient client, IConfiguration config)
    {
        this.client = client;
        notificationsEndpoint = config["NotificationService:Endpoint"] ?? "/api/notifications";
    }
    
    public async Task SendNotificationAsync(NotificationRequest request)
    {
        var responce = await client
            .PostAsJsonAsync(notificationsEndpoint,request);
        
        if (!responce.IsSuccessStatusCode)
        {
            throw new NotificationSendException();
        }
    }
}