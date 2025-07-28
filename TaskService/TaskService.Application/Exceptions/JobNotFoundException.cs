namespace TaskService.Application.Exceptions;

public class JobNotFoundException : Exception
{
    public JobNotFoundException(Guid jobId) : base($"Job with id {jobId} not found"){}
}