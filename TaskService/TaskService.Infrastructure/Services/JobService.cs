using Microsoft.EntityFrameworkCore;
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

    public async Task<PagedResult<JobResponse>> GetJobsAsync(GetJobsRequest request)
    {
        var query = dbContext.Jobs
            .AsNoTracking()
            .Where(j => !j.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(j =>
                j.Title.ToLower().Contains(request.Search.ToLower()) ||
                (j.Description != null && j.Description.ToLower().Contains(request.Search.ToLower())));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(j => j.Status == request.Status.Value);
        }
        
        int totalCount = await query.CountAsync();

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var items = jobs.Select(job => new JobResponse(
            job.Id,
            job.Title,
            job.Description,
            job.Status,
            job.CreatorId,
            job.AssigneeId,
            job.CreatedAt
        )).ToList();

        return new PagedResult<JobResponse>(totalCount, items);
    }

    public async Task<JobResponse?> GetJobByIdAsync(Guid id)
    {
        Job? job = await dbContext.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
        
        if (job is null) return null;

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