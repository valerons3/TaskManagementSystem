using TaskService.Application.Contracts.Jobs;

namespace TaskService.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse> CreateJobAsync(CreateJobRequest request, Guid creatorId);
    Task<PagedResult<JobResponse>> GetJobsAsync(GetJobsRequest request);
    Task<JobResponse?> GetJobByIdAsync(Guid id);
    Task<bool> UpdateJobAsync(Guid id, UpdateJobRequest request);
    Task<bool> DeleteJobAsync(Guid id);
    Task<bool> AssignJobAsync(Guid jobId, Guid assigneeId);
}