using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> connections = new();
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userId, out var guid))
        {
            var connectionId = Context.ConnectionId;
            connections.AddOrUpdate(guid,
                _ => new HashSet<string> { connectionId },
                (_, set) => { set.Add(connectionId); return set; });
        }

        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userId, out var guid))
        {
            if (connections.TryGetValue(guid, out var set))
            {
                set.Remove(Context.ConnectionId);
                if (set.Count == 0)
                    connections.TryRemove(guid, out _);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
    
    public static async Task SendNotificationToUser(IHubContext<NotificationHub> hubContext, Guid userId, string message)
    {
        if (connections.TryGetValue(userId, out var connectionIds))
        {
            foreach (var connectionId in connectionIds)
            {
                await hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}