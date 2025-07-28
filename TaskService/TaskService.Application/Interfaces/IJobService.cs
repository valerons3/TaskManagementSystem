using TaskService.Application.Contracts.Jobs;

namespace TaskService.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse> CreateJobAsync(CreateJobRequest request, Guid creatorId);
    Task<PagedResult<JobResponse>> GetJobsAsync(GetJobsRequest request);
    Task<JobResponse> GetJobByIdAsync(Guid id);
    Task UpdateJobAsync(Guid id, UpdateJobRequest request);
    Task DeleteJobAsync(Guid id);
    Task AssignJobAsync(Guid jobId, Guid assigneeId);
}