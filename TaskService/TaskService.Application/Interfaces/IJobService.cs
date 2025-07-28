using TaskService.Application.Contracts.Jobs;

namespace TaskService.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse> CreateJobAsync(CreateJobRequest request, Guid creatorId);
}