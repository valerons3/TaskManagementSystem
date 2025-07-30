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
    public async Task<IActionResult> Create([FromBody] NotificationRequest request)
    {
        await service.CreateNotificationAsync(request);
        return StatusCode(201); 
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var notifications = await service.GetNotificationsByUserIdAsync(userId);
        return Ok(notifications);
    }

    [HttpPut("{id:guid}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await service.MarkAsReadAsync(id);
        return NoContent();
    }
}