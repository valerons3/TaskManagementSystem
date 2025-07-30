using System.Net.Http.Json;
using TaskService.Application.Contracts.Notifications;
using TaskService.Application.Interfaces;

namespace TaskService.Infrastructure.Services;

public class NotificationClient : INotificationClient
{
    private readonly HttpClient client;

    public NotificationClient(HttpClient client)
    {
        this.client = client;
    }
    
    public async Task SendNotificationAsync(NotificationRequest request)
    {
        var responce = await client
            .PostAsJsonAsync("/api/notifications",request);
        
        if (!responce.IsSuccessStatusCode)
        {
            throw new Exception("Failed to send notification");
        }
    }
}