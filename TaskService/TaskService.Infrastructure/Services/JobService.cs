using TaskService.Application.Contracts.Jobs;
using TaskService.Application.Interfaces;
using TaskService.Domain.Entities;
using TaskService.Domain.Enums;
using TaskService.Persistence.DbContexts;

namespace TaskService.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly TaskDbContext dbContext;

    public JobService(TaskDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    public async Task<JobResponse> CreateJobAsync(CreateJobRequest request, Guid creatorId)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Title = request.Title,
            Description = request.Description,
            Status = JobStatus.Todo,
            CreatedAt = DateTime.UtcNow,
        };

        await dbContext.Jobs.AddAsync(job);
        await dbContext.SaveChangesAsync();

        return new JobResponse(
            job.Id,
            job.Title,
            job.Description,
            job.Status,
            job.CreatorId,
            job.AssigneeId,
            job.CreatedAt
        );
    }
}