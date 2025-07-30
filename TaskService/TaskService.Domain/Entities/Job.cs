using TaskService.Domain.Enums;

namespace TaskService.Domain.Entities;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CreatorId { get; set; }
    public Guid? AssigneeId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Todo;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = default!;
}