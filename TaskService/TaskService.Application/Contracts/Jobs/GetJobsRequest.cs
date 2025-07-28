using TaskService.Domain.Enums;

namespace TaskService.Application.Contracts.Jobs;

public record GetJobsRequest(
    string? Search,
    JobStatus? Status,
    int Page = 1,
    int PageSize = 10);