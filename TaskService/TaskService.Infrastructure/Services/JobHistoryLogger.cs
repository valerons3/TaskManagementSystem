using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.Repositories;
using TaskService.Domain.Entities;
using TaskService.Persistence.DbContexts;

namespace TaskService.Infrastructure.Services;

public class JobHistoryLogger : IJobHistoryLogger
{
    private readonly IJobHistoryRepository jobHistoryRepository;

    public JobHistoryLogger(IJobHistoryRepository jobHistoryRepository)
    {
        this.jobHistoryRepository = jobHistoryRepository;
    }
    
    public async Task LogHistoryAsync(Guid jobId, string action, string? performedBy)
    {
        var history = new JobHistory
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            Action = action,
            Timestamp = DateTime.UtcNow,
            PerformedBy = performedBy
        };

        await jobHistoryRepository.AddAsync(history);
    }
}