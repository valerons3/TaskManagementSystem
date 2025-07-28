namespace TaskService.Application.Interfaces;

public interface IJobHistoryService
{
    Task LogHistoryAsync(Guid jobId, string action, string? performedBy);
}