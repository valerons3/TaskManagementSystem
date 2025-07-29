using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Contracts;
using NotificationService.Application.Interfaces;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("/api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService service;

    public NotificationsController(INotificationService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationRequest request)
    {
        await service.CreateNotificationAsync(request);
        return Created();
    }
    
    
}