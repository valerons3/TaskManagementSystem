using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
    Task<Notification?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}