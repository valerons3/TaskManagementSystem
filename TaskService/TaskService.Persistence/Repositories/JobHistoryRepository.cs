using TaskService.Application.Interfaces.Repositories;
using TaskService.Domain.Entities;
using TaskService.Persistence.DbContexts;

namespace TaskService.Persistence.Repositories;

public class JobHistoryRepository : IJobHistoryRepository
{
    private readonly JobDbContext dbContext;

    public JobHistoryRepository(JobDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(JobHistory history)
    {
        await dbContext.JobHistories.AddAsync(history);
        await dbContext.SaveChangesAsync();
    }
}