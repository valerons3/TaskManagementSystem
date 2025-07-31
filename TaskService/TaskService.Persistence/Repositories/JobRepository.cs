using Microsoft.EntityFrameworkCore;
using TaskService.Application.Interfaces.Repositories;
using TaskService.Domain.Entities;
using TaskService.Domain.Enums;
using TaskService.Persistence.DbContexts;

namespace TaskService.Persistence.Repositories;

public class JobRepository : IJobRepository
{
    private readonly JobDbContext dbContext;

    public JobRepository(JobDbContext dbContext)
    {
        this.dbContext = dbContext;
    }


    public async Task AddAsync(Job job)
    {
        await dbContext.Jobs.AddAsync(job);
    }

    public async Task<Job?> GetByIdAsync(Guid jobId)
    {
        return await dbContext.Jobs
            .FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDeleted);
    }

    public async Task<List<Job>> GetUserJobsAsync(Guid userId, string? search, JobStatus? status, int skip, int take)
    {
        var query = dbContext.Jobs
            .AsNoTracking()
            .Where(j => !j.IsDeleted && (j.CreatorId == userId || j.AssigneeId == userId));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(j =>
                j.Title.ToLower().Contains(search.ToLower()) ||
                (j.Description != null && j.Description.ToLower().Contains(search.ToLower())));
        }

        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        return await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetUserJobsCountAsync(Guid userId, string? search, JobStatus? status)
    {
        var query = dbContext.Jobs
            .Where(j => !j.IsDeleted && (j.CreatorId == userId || j.AssigneeId == userId));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(j =>
                j.Title.ToLower().Contains(search.ToLower()) ||
                (j.Description != null && j.Description.ToLower().Contains(search.ToLower())));
        }

        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        return await query.CountAsync();
    }
    
    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}