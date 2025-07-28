using Microsoft.EntityFrameworkCore;
using TaskService.Application.Contracts.Jobs;
using TaskService.Application.Exceptions;
using TaskService.Application.Interfaces;
using TaskService.Domain.Entities;
using TaskService.Domain.Enums;
using TaskService.Persistence.DbContexts;

namespace TaskService.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly JobDbContext dbContext;
    private readonly IJobHistoryService jobHistoryService;

    public JobService(JobDbContext dbContext, IJobHistoryService jobHistoryService)
    {
        this.dbContext = dbContext;
        this.jobHistoryService = jobHistoryService;
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
        await jobHistoryService.LogHistoryAsync(job.Id, "Job created", null);
        
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

    public async Task<JobResponse> GetJobByIdAsync(Guid id)
    {
        Job? job = await dbContext.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
        
        if (job is null)
            throw new JobNotFoundException(id);

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

    public async Task UpdateJobAsync(Guid id, UpdateJobRequest request)
    {
        Job? job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
        if (job is null)
            throw new JobNotFoundException(id);

        if (job is null)
            throw new JobNotFoundException(id);

        job.Title = request.Title;

        if (request.Description is not null)
            job.Description = request.Description;
        if (request.Status is not null)
            job.Status = request.Status.Value;

        await dbContext.SaveChangesAsync();
        await jobHistoryService.LogHistoryAsync(job.Id, "Job updated", null);
    }

    public async Task DeleteJobAsync(Guid id)
    {
        Job? job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
        if (job is null) 
            throw new JobNotFoundException(id);
        
        job.IsDeleted = true;
        await dbContext.SaveChangesAsync();
        await jobHistoryService.LogHistoryAsync(job.Id, "Job deleted", null);
    }

    public async Task AssignJobAsync(Guid jobId, Guid assigneeId)
    {
        Job? job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDeleted);
        if (job is null)
            throw new JobNotFoundException(jobId);

        job.AssigneeId = assigneeId;
        await dbContext.SaveChangesAsync();
        await jobHistoryService.LogHistoryAsync(job.Id, $"Assigned to user {assigneeId}", null);
    }
}