using TaskService.Domain.Enums;

namespace TaskService.Application.Contracts.Jobs;

public record UpdateJobRequest(string Title,
    string? Description,
    JobStatus? Status);