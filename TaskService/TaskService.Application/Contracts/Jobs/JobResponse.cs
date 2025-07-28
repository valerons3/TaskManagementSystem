using TaskService.Domain.Enums;

namespace TaskService.Application.Contracts.Jobs;

public record JobResponse(
    Guid Id,
    string Title,
    string? Description,
    JobStatus Status,
    Guid CreatorId,
    Guid? AssigneeId,
    DateTime CreatedAt);