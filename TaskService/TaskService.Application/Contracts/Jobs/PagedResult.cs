namespace TaskService.Application.Contracts.Jobs;

public record PagedResult<T>(int TotalCount,
    IReadOnlyList<T> Items);