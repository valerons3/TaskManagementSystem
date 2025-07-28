namespace TaskService.Domain.Entities;

public class JobHistory
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string Action { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? PerformedBy { get; set; }
}