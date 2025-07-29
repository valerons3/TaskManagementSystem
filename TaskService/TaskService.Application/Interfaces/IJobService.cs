using TaskService.Application.Contracts.Jobs;

namespace TaskService.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse> CreateJobAsync(CreateJobRequest request, Guid creatorId);
    Task<PagedResult<JobResponse>> GetJobsAsync(GetJobsRequest request, Guid userId);
    Task<JobResponse> GetJobByIdAsync(Guid jobId, Guid userId);
    Task UpdateJobAsync(Guid jobId, Guid userId,UpdateJobRequest request);
    Task DeleteJobAsync(Guid jobId, Guid userId);
    Task AssignJobAsync(Guid jobId, Guid assigneeId, Guid userId);
}