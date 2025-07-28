using TaskService.Application.Interfaces;
using TaskService.Domain.Entities;
using TaskService.Persistence.DbContexts;

namespace TaskService.Infrastructure.Services;

public class JobHistoryService : IJobHistoryService
{
    private readonly JobDbContext dbContext;

    public JobHistoryService(JobDbContext dbContext)
    {
        this.dbContext = dbContext;
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

        await dbContext.JobHistories.AddAsync(history);
        await dbContext.SaveChangesAsync();
    }
}