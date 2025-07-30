namespace TaskService.Application.Interfaces;

public interface IJobHistoryLogger
{
    Task LogHistoryAsync(Guid jobId, string action, string? performedBy);
}