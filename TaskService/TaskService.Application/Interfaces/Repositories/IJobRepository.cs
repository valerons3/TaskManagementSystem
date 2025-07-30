using TaskService.Domain.Entities;
using TaskService.Domain.Enums;

namespace TaskService.Application.Interfaces.Repositories;

public interface IJobRepository
{
    Task AddAsync(Job job);
    Task<Job?> GetByIdAsync(Guid jobId);
    Task<List<Job>> GetUserJobsAsync(Guid userId, string? search, JobStatus? status, int skip, int take);
    Task<int> GetUserJobsCountAsync(Guid userId, string? search, JobStatus? status);
    Task SaveChangesAsync();
}