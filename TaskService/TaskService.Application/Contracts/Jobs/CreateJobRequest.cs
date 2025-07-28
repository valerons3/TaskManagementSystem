namespace TaskService.Application.Contracts.Jobs;

public record CreateJobRequest(string Title,
    string? Description);